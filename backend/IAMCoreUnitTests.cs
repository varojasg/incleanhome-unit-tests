using InCleanHome.API.IAM.Application.Internal.CommandServices;
using InCleanHome.API.IAM.Application.Internal.OutboundServices;
using InCleanHome.API.IAM.Domain.Model.Aggregates;
using InCleanHome.API.IAM.Domain.Model.Commands;
using InCleanHome.API.IAM.Domain.Model.ValueObjects;
using InCleanHome.API.IAM.Domain.Repositories;
using InCleanHome.API.Shared.Domain.Repositories;

namespace InCleanHome.Tests;

public class IAMCoreUnitTests
{
    // US01 - Registro de usuario según rol
    [Fact]
    public async Task TestSignUp_UsuarioValido()
    {
        //Arrange
        var repository = new FakeUserRepository();
        var hashing = new FakeHashingService();
        var service = CreateService(repository, hashing);
        var command = new SignUpCommand("cliente@correo.com", "Clave123", UserRole.Client);

        //Act
        var resultado = await service.Handle(command);

        //Assert
        Assert.Equal("cliente@correo.com", resultado.Email);
        Assert.Equal("hash:Clave123", resultado.PasswordHash);
        Assert.Equal(UserRole.Client, resultado.Role);
    }

    // US01 - Registro de usuario según rol
    [Fact]
    public async Task TestSignUp_EmailDuplicado()
    {
        //Arrange
        var repository = new FakeUserRepository();
        await repository.AddAsync(new User("cliente@correo.com", "hash:Clave123", UserRole.Client));
        var service = CreateService(repository, new FakeHashingService());
        var command = new SignUpCommand("cliente@correo.com", "Otra123", UserRole.Client);

        //Act
        var accion = () => service.Handle(command);

        //Assert
        var error = await Assert.ThrowsAsync<Exception>(accion);
        Assert.Contains("already taken", error.Message);
    }

    // US02 - Gestionar inicio de sesión
    [Fact]
    public async Task TestSignIn_ContrasenaIncorrecta()
    {
        //Arrange
        var repository = new FakeUserRepository();
        var usuario = new User("cliente@correo.com", "hash:Correcta123", UserRole.Client)
            .AcceptTerms(TermsVersion.CurrentVersion);
        await repository.AddAsync(usuario);
        var service = CreateService(repository, new FakeHashingService());
        var command = new SignInCommand("cliente@correo.com", "Incorrecta123");

        //Act
        var accion = () => service.Handle(command);

        //Assert
        var error = await Assert.ThrowsAsync<Exception>(accion);
        Assert.Contains("Invalid email or password", error.Message);
    }

    // US02 - Gestionar inicio de sesión
    [Fact]
    public async Task TestSignIn_CredencialesCorrectasContinuaFlujo()
    {
        //Arrange
        var repository = new FakeUserRepository();
        var usuario = new User("cliente@correo.com", "hash:Clave123", UserRole.Client)
            .AcceptTerms(TermsVersion.CurrentVersion);
        await repository.AddAsync(usuario);
        var service = CreateService(repository, new FakeHashingService());

        //Act
        var resultado = await service.Handle(new SignInCommand("cliente@correo.com", "Clave123"));

        //Assert
        Assert.True(resultado.Requires2faSetup);
        Assert.False(resultado.Requires2fa);
        Assert.NotNull(resultado.ChallengeToken);
    }

    // US04 - Autenticación de dos factores
    [Fact]
    public async Task TestSignIn_UsuarioCon2FARequiereCodigo()
    {
        //Arrange
        var repository = new FakeUserRepository();
        var usuario = new User("worker@correo.com", "hash:Clave123", UserRole.Worker)
            .AcceptTerms(TermsVersion.CurrentVersion)
            .SetTotpSecret("SECRETO")
            .EnableTotp();
        await repository.AddAsync(usuario);
        var service = CreateService(repository, new FakeHashingService());

        //Act
        var resultado = await service.Handle(new SignInCommand("worker@correo.com", "Clave123"));

        //Assert
        Assert.True(resultado.Requires2fa);
        Assert.False(resultado.Requires2faSetup);
        Assert.NotNull(resultado.ChallengeToken);
    }

    // US04 - Autenticación de dos factores
    [Fact]
    public void TestUser_Desactivar2FAEliminaSecreto()
    {
        //Arrange
        var usuario = new User("worker@correo.com", "hash", UserRole.Worker)
            .SetTotpSecret("SECRETO")
            .EnableTotp();

        //Act
        usuario.DisableTotp();

        //Assert
        Assert.False(usuario.TotpEnabled);
        Assert.Null(usuario.TotpSecret);
    }

    private static UserCommandService CreateService(FakeUserRepository repository, FakeHashingService hashing)
    {
        return new UserCommandService(
            repository,
            new FakeTokenService(),
            new FakeChallengeTokenService(),
            hashing,
            new FakeUnitOfWork());
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        private readonly List<User> users = new();

        public Task AddAsync(User entity) { users.Add(entity); return Task.CompletedTask; }
        public Task<User?> FindByIdAsync(int id) => Task.FromResult(users.FirstOrDefault(x => x.Id == id));
        public void Update(User entity) { }
        public void Remove(User entity) => users.Remove(entity);
        public Task<IEnumerable<User>> ListAsync() => Task.FromResult<IEnumerable<User>>(users);
        public Task<User?> FindByEmailAsync(string email) => Task.FromResult(users.FirstOrDefault(x => x.Email == email));
        public bool ExistsByEmail(string email) => users.Any(x => x.Email == email);
    }

    private sealed class FakeHashingService : IHashingService
    {
        public string HashPassword(string password) => $"hash:{password}";
        public bool VerifyPassword(string password, string passwordHash) => passwordHash == $"hash:{password}";
    }

    private sealed class FakeTokenService : ITokenService
    {
        public string GenerateToken(User user) => "token";
        public Task<int?> ValidateToken(string token) => Task.FromResult<int?>(0);
    }

    private sealed class FakeChallengeTokenService : IChallengeTokenService
    {
        public string IssueChallengeToken(int userId, string purpose) => $"challenge:{purpose}";
        public Task<int?> ValidateChallengeToken(string token, string purpose) => Task.FromResult<int?>(0);
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public Task CompleteAsync() => Task.CompletedTask;
    }
}
