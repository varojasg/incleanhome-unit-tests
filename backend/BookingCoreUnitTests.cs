using InCleanHome.API.Booking.Application.Internal.CommandServices;
using InCleanHome.API.Booking.Domain.Model.Aggregates;
using InCleanHome.API.Booking.Domain.Model.Commands;
using InCleanHome.API.Booking.Domain.Model.ValueObjects;
using InCleanHome.API.IAM.Domain.Model.ValueObjects;

namespace InCleanHome.Tests;

public class BookingCoreUnitTests
{
    // US10 - Reservar servicio
    [Fact]
    public void TestBookingRequest_CalculaMontoReferencial()
    {
        //Arrange
        const decimal tarifa = 30m;
        const decimal horas = 2.5m;

        //Act
        var reserva = CrearReserva(tarifa, horas);

        //Assert
        Assert.Equal(75m, reserva.TotalAmount);
        Assert.Equal(BookingStatus.Pending, reserva.Status);
    }

    // US10 - Reservar servicio
    [Fact]
    public async Task TestCreateBooking_FechaPasada()
    {
        //Arrange
        var service = new BookingRequestCommandService(null!, null!, null!, null!);
        var command = new CreateBookingCommand(1, 2, "cleaning",
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)), "09:00", "11:00", 2m, "Av. Lima 100", null);

        //Act
        var accion = () => service.Handle(command);

        //Assert
        await Assert.ThrowsAsync<ArgumentException>(accion);
    }

    // US11 - Gestionar reservas como cliente
    [Fact]
    public void TestCancelByClient_ReservaPendiente()
    {
        //Arrange
        var reserva = CrearReserva();

        //Act
        reserva.CancelByClient();

        //Assert
        Assert.Equal(BookingStatus.Cancelled, reserva.Status);
    }

    // US11 - Gestionar reservas como cliente
    [Fact]
    public void TestCancelByClient_ReservaFinalizada()
    {
        //Arrange
        var reserva = CrearReserva();
        reserva.Accept().Complete();

        //Act
        var accion = reserva.CancelByClient;

        //Assert
        Assert.Throws<InvalidOperationException>(accion);
    }

    // US12 - Gestionar solicitudes como prestador
    [Fact]
    public void TestAccept_ReservaPendiente()
    {
        //Arrange
        var reserva = CrearReserva();

        //Act
        reserva.Accept();

        //Assert
        Assert.Equal(BookingStatus.Accepted, reserva.Status);
    }

    // US12 - Gestionar solicitudes como prestador
    [Fact]
    public void TestComplete_ReservaAceptada()
    {
        //Arrange
        var reserva = CrearReserva();
        reserva.Accept();

        //Act
        reserva.Complete();

        //Assert
        Assert.Equal(BookingStatus.Completed, reserva.Status);
    }

    private static BookingRequest CrearReserva(decimal tarifa = 25m, decimal horas = 2m)
    {
        return new BookingRequest(1, 2, "cleaning",
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), "09:00", "11:00", horas,
            "Av. Lima 100", "", tarifa);
    }
}
