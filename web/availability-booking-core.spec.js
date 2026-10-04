import { describe, it, expect } from "vitest";
import { Availability } from "../src/Booking/domain/model/availability.entity.js";
import { Booking } from "../src/Booking/domain/model/booking.entity.js";
import { BookingStatus } from "../src/Booking/domain/model/booking-status.value.js";
import { BookingPricingService } from "../src/Booking/domain/services/booking-pricing.service.js";
import { notPast, timeAfter } from "../src/Shared/domain/validation/validators.js";

describe("Availability and booking core unit tests", () => {
  // US09 - Gestionar disponibilidad
  it("testAvailability_SemanaPorDefecto", () => {
    //Arrange

    //Act
    const semana = Availability.defaultWeek();

    //Assert
    expect(semana).toHaveLength(7);
    expect(semana[1].isAvailable).toBe(true);
    expect(semana[0].isAvailable).toBe(false);
  });

  // US09 - Gestionar disponibilidad
  it("testAvailability_DiaNoDisponible", () => {
    //Arrange
    const raw = { dayOfWeek: 6, startTime: "10:00", endTime: "14:00", isAvailable: false };

    //Act
    const disponibilidad = Availability.fromApi(raw);

    //Assert
    expect(disponibilidad.dayOfWeek).toBe(6);
    expect(disponibilidad.isAvailable).toBe(false);
  });

  // US10 - Reservar servicio
  it("testBookingPricing_CalculaMonto", () => {
    //Arrange
    const tarifa = 30;
    const horas = 2.5;

    //Act
    const total = BookingPricingService.calculateTotal(tarifa, horas);

    //Assert
    expect(total).toBe(75);
  });

  // US10 - Reservar servicio
  it("testBooking_FechaYHorarioValidos", () => {
    //Arrange
    const ahora = () => new Date(2026, 9, 3, 10, 0);
    const validarFecha = notPast(undefined, ahora);
    const validarFin = timeAfter("startTime");

    //Act
    const fechaValida = validarFecha("2026-10-04");
    const fechaPasada = validarFecha("2026-10-02");
    const horaValida = validarFin("11:00", { startTime: "09:00" });

    //Assert
    expect(fechaValida).toBeNull();
    expect(fechaPasada).not.toBeNull();
    expect(horaValida).toBeNull();
  });

  // US11 - Gestionar reservas como cliente
  it("testBooking_PendientePuedeCancelarse", () => {
    //Arrange
    const reserva = new Booking({ status: BookingStatus.PENDING });

    //Act
    const puedeCancelar = reserva.canBeCancelled();

    //Assert
    expect(puedeCancelar).toBe(true);
  });

  // US11 - Gestionar reservas como cliente
  it("testBooking_AceptadaNoPuedeCancelarseDesdeFrontend", () => {
    //Arrange
    const reserva = new Booking({ status: BookingStatus.ACCEPTED });

    //Act
    const puedeCancelar = reserva.canBeCancelled();

    //Assert
    expect(puedeCancelar).toBe(false);
  });

  // US12 - Gestionar solicitudes como prestador
  it("testBooking_IdentificaReservaAceptada", () => {
    //Arrange
    const reserva = new Booking({ status: BookingStatus.ACCEPTED });

    //Act
    const aceptada = reserva.isAccepted();

    //Assert
    expect(aceptada).toBe(true);
  });

  // US12 - Gestionar solicitudes como prestador
  it("testBooking_IdentificaReservaCompletada", () => {
    //Arrange
    const reserva = new Booking({ status: BookingStatus.COMPLETED });

    //Act
    const completada = reserva.isCompleted();

    //Assert
    expect(completada).toBe(true);
  });
});
