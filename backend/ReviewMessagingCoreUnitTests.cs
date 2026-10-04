using InCleanHome.API.Messaging.Application.Internal.CommandServices;
using InCleanHome.API.Messaging.Domain.Model.Aggregates;
using InCleanHome.API.Messaging.Domain.Model.Commands;
using InCleanHome.API.ReviewsAndEvaluation.Domain.Model.Aggregates;

namespace InCleanHome.Tests;

public class ReviewMessagingCoreUnitTests
{
    // US13 - Calificar servicio completado
    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    public void TestReview_CalificacionValida(int calificacion)
    {
        //Arrange
        const int reservaId = 10;

        //Act
        var review = new Review(reservaId, 1, 2, calificacion, "Buen servicio");

        //Assert
        Assert.Equal(calificacion, review.Rating);
    }

    // US13 - Calificar servicio completado
    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void TestReview_CalificacionFueraDeRango(int calificacion)
    {
        //Arrange
        var accion = () => new Review(10, 1, 2, calificacion, "Comentario");

        //Act
        var error = Record.Exception(accion);

        //Assert
        Assert.IsType<ArgumentException>(error);
    }

    // US14 - Gestionar mensajería directa
    [Fact]
    public void TestMessage_MarcarComoLeido()
    {
        //Arrange
        var mensaje = new Message(1, 2, "Hola");

        //Act
        mensaje.MarkAsRead();
        var primeraLectura = mensaje.ReadAt;
        mensaje.MarkAsRead();

        //Assert
        Assert.NotNull(mensaje.ReadAt);
        Assert.Equal(primeraLectura, mensaje.ReadAt);
    }

    // US14 - Gestionar mensajería directa
    [Fact]
    public async Task TestSendMessage_MensajeVacio()
    {
        //Arrange
        var service = new MessageCommandService(null!, null!);
        var command = new SendMessageCommand(1, 2, "   ");

        //Act
        var accion = () => service.Handle(command);

        //Assert
        var error = await Assert.ThrowsAsync<Exception>(accion);
        Assert.Contains("cannot be empty", error.Message);
    }
}
