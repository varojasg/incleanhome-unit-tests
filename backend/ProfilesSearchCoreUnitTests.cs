using InCleanHome.API.Profiles.Application.Internal.QueryServices;
using InCleanHome.API.Profiles.Domain.Model.Aggregates;
using InCleanHome.API.Profiles.Domain.Model.Queries;
using InCleanHome.API.Profiles.Domain.Repositories;

namespace InCleanHome.Tests;

public class ProfilesSearchCoreUnitTests
{
    // US05 - Gestionar perfil
    [Fact]
    public void TestClientProfile_UpdateDatosValidos()
    {
        //Arrange
        var perfil = new ClientProfile(1, "Ana", "987654321");

        //Act
        perfil.Update("Ana María", "999888777");

        //Assert
        Assert.Equal("Ana María", perfil.Name);
        Assert.Equal("999888777", perfil.Phone);
    }

    // US05 - Gestionar perfil
    [Fact]
    public void TestClientProfile_NombreVacio()
    {
        //Arrange
        var perfil = new ClientProfile(1, "Ana", "987654321");

        //Act
        var accion = () => perfil.Update("   ", "999888777");

        //Assert
        Assert.Throws<ArgumentException>(accion);
    }

    // US06 - Buscar prestadores de servicios
    [Fact]
    public async Task TestSearchWorkers_EnviaFiltrosAlRepositorio()
    {
        //Arrange
        var repository = new FakeWorkerProfileRepository();
        var service = new WorkerProfileQueryService(repository);
        var query = new SearchWorkersQuery("cleaning", "miraflores", "female", 20, 50, 40m, 4m);

        //Act
        await service.Handle(query);

        //Assert
        Assert.Equal(query, repository.LastSearch);
    }

    // US06 - Buscar prestadores de servicios
    [Fact]
    public async Task TestSearchWorkers_DevuelveResultadosDelRepositorio()
    {
        //Arrange
        var esperado = new WorkerProfile(2, "María", "987654321", 30, "female",
            new List<string> { "cleaning" }, new List<string> { "miraflores" }, 30m, 4, "Experiencia");
        var repository = new FakeWorkerProfileRepository { SearchResults = new[] { esperado } };
        var service = new WorkerProfileQueryService(repository);

        //Act
        var resultado = (await service.Handle(new SearchWorkersQuery(null, null, null, null, null, null, null))).ToList();

        //Assert
        Assert.Single(resultado);
        Assert.Equal("María", resultado[0].Name);
    }

    // US07 - Consultar perfil de prestador
    [Fact]
    public void TestWorkerProfile_ConservaInformacionDelPrestador()
    {
        //Arrange
        var servicios = new List<string> { "cleaning", "laundry" };
        var zonas = new List<string> { "san-isidro" };

        //Act
        var perfil = new WorkerProfile(2, "María", "987654321", 32, "female",
            servicios, zonas, 35m, 5, "Trabajo en hogares");

        //Assert
        Assert.Equal(servicios, perfil.ServiceTypes);
        Assert.Equal(zonas, perfil.Zones);
        Assert.Equal(35m, perfil.HourlyRate);
    }

    // US07 - Consultar perfil de prestador
    [Fact]
    public void TestWorkerProfile_RegistraPromedioDeCalificacion()
    {
        //Arrange
        var perfil = new WorkerProfile(2, "María", "987654321", 32, "female",
            new List<string> { "cleaning" }, new List<string> { "san-isidro" }, 35m, 5, "");

        //Act
        perfil.RegisterCompletedService(5);
        perfil.RegisterCompletedService(3);

        //Assert
        Assert.Equal(2, perfil.TotalServices);
        Assert.Equal(4m, perfil.AverageRating);
    }

    private sealed class FakeWorkerProfileRepository : IWorkerProfileRepository
    {
        public SearchWorkersQuery? LastSearch { get; private set; }
        public IEnumerable<WorkerProfile> SearchResults { get; init; } = Array.Empty<WorkerProfile>();

        public Task<IEnumerable<WorkerProfile>> SearchAsync(SearchWorkersQuery filters)
        {
            LastSearch = filters;
            return Task.FromResult(SearchResults);
        }

        public Task<WorkerProfile?> FindByUserIdAsync(int userId) => Task.FromResult<WorkerProfile?>(null);
        public Task AddAsync(WorkerProfile entity) => Task.CompletedTask;
        public Task<WorkerProfile?> FindByIdAsync(int id) => Task.FromResult<WorkerProfile?>(null);
        public void Update(WorkerProfile entity) { }
        public void Remove(WorkerProfile entity) { }
        public Task<IEnumerable<WorkerProfile>> ListAsync() => Task.FromResult<IEnumerable<WorkerProfile>>(Array.Empty<WorkerProfile>());
    }
}
