using InCleanHome.API.SearchAndCatalog.Domain.Model.Aggregates;

namespace InCleanHome.Tests;

public class AvailabilityCoreUnitTests
{
    // US09 - Gestionar disponibilidad
    [Fact]
    public void TestAvailabilitySlot_DiaDisponible()
    {
        //Arrange
        const int trabajadorId = 5;

        //Act
        var disponibilidad = new AvailabilitySlot(trabajadorId, 1, "08:00", "18:00", true);

        //Assert
        Assert.True(disponibilidad.IsAvailable);
        Assert.Equal("08:00", disponibilidad.StartTime);
        Assert.Equal("18:00", disponibilidad.EndTime);
    }

    // US09 - Gestionar disponibilidad
    [Fact]
    public void TestAvailabilitySlot_ActualizarDiaNoDisponible()
    {
        //Arrange
        var disponibilidad = new AvailabilitySlot(5, 1, "08:00", "18:00", true);

        //Act
        disponibilidad.Update("09:00", "17:00", false);

        //Assert
        Assert.False(disponibilidad.IsAvailable);
        Assert.Equal("09:00", disponibilidad.StartTime);
        Assert.Equal("17:00", disponibilidad.EndTime);
    }
}
