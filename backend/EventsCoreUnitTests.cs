using InCleanHome.API.Events.Domain.Model.Aggregates;
using InCleanHome.API.Events.Domain.Model.ValueObjects;
using EventAggregate = InCleanHome.API.Events.Domain.Model.Aggregates.Event;

namespace InCleanHome.Tests;

public class EventsCoreUnitTests
{
    // US15 - Crear evento de servicios para el hogar
    [Fact]
    public void TestEvent_CreacionValidaEstadoAbierto()
    {
        //Arrange
        var servicios = new List<string> { "cleaning" };

        //Act
        var evento = CrearEvento(servicios: servicios, trabajadores: 2);

        //Assert
        Assert.Equal(EventStatus.Open, evento.Status);
        Assert.Equal(2, evento.WorkersNeeded);
    }

    // US15 - Crear evento de servicios para el hogar
    [Fact]
    public void TestEvent_SinServicios()
    {
        //Arrange
        var accion = () => CrearEvento(servicios: new List<string>());

        //Act
        var error = Record.Exception(accion);

        //Assert
        Assert.IsType<ArgumentException>(error);
    }

    // US16 - Gestionar eventos publicados
    [Fact]
    public void TestEvent_CancelarEventoAbierto()
    {
        //Arrange
        var evento = CrearEvento();

        //Act
        evento.Cancel();

        //Assert
        Assert.Equal(EventStatus.Cancelled, evento.Status);
    }

    // US16 - Gestionar eventos publicados
    [Fact]
    public void TestEvent_CompletarAntesDeFecha()
    {
        //Arrange
        var evento = CrearEvento(fecha: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)));
        evento.MarkStaffed();

        //Act
        var accion = evento.Complete;

        //Assert
        Assert.Throws<InvalidOperationException>(accion);
    }

    // US17 - Gestionar postulaciones recibidas en un evento
    [Fact]
    public void TestEventApplication_AceptarPostulacionPendiente()
    {
        //Arrange
        var postulacion = new EventApplication(10, 20, "Tengo experiencia");

        //Act
        postulacion.Accept();

        //Assert
        Assert.Equal(ApplicationStatus.Accepted, postulacion.Status);
    }

    // US17 - Gestionar postulaciones recibidas en un evento
    [Fact]
    public void TestEvent_MarcarPersonalCompleto()
    {
        //Arrange
        var evento = CrearEvento(trabajadores: 1);

        //Act
        evento.MarkStaffed();

        //Assert
        Assert.Equal(EventStatus.Staffed, evento.Status);
        Assert.False(evento.CanAcceptApplications());
    }

    // US19 - Postular a un evento
    [Fact]
    public void TestEventApplication_RetirarPostulacionPendiente()
    {
        //Arrange
        var postulacion = new EventApplication(10, 20, null);

        //Act
        postulacion.Withdraw();

        //Assert
        Assert.Equal(ApplicationStatus.Withdrawn, postulacion.Status);
    }

    // US19 - Postular a un evento
    [Fact]
    public void TestEventApplication_RetirarPostulacionAceptada()
    {
        //Arrange
        var postulacion = new EventApplication(10, 20, null);
        postulacion.Accept();

        //Act
        var accion = postulacion.Withdraw;

        //Assert
        Assert.Throws<InvalidOperationException>(accion);
    }

    private static EventAggregate CrearEvento(
        List<string>? servicios = null,
        int trabajadores = 1,
        DateOnly? fecha = null)
    {
        var eventDate = fecha ?? DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3));
        return new EventAggregate(
            1,
            "Limpieza general",
            "Evento de prueba",
            servicios ?? new List<string> { "cleaning" },
            "san-isidro",
            "Av. Principal 100",
            eventDate,
            "09:00",
            "12:00",
            3m,
            trabajadores,
            30m,
            DateTime.UtcNow.AddDays(1));
    }
}
