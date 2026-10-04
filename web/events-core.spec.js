import { describe, it, expect } from "vitest";
import { Event } from "../src/Events/domain/model/event.entity.js";
import { EventStatus } from "../src/Events/domain/model/event-status.value.js";
import { EventApplication } from "../src/Events/domain/model/event-application.entity.js";
import { ApplicationStatus } from "../src/Events/domain/model/application-status.value.js";
import { minItems, min, notPast, timeAfter } from "../src/Shared/domain/validation/validators.js";

describe("Events core unit tests", () => {
  // US15 - Crear evento de servicios para el hogar
  it("testCrearEvento_RequiereServicioYTrabajador", () => {
    //Arrange
    const validarServicios = minItems(1);
    const validarTrabajadores = min(1);

    //Act
    const sinServicios = validarServicios([]);
    const conServicio = validarServicios(["cleaning"]);
    const sinTrabajadores = validarTrabajadores(0);

    //Assert
    expect(sinServicios).not.toBeNull();
    expect(conServicio).toBeNull();
    expect(sinTrabajadores).not.toBeNull();
  });

  // US15 - Crear evento de servicios para el hogar
  it("testCrearEvento_FechaYHorarioValidos", () => {
    //Arrange
    const ahora = () => new Date(2026, 9, 3, 10, 0);
    const validarFecha = notPast(undefined, ahora);
    const validarFin = timeAfter("startTime");

    //Act
    const fechaValida = validarFecha("2026-10-05");
    const fechaPasada = validarFecha("2026-10-02");
    const horaValida = validarFin("15:00", { startTime: "09:00" });

    //Assert
    expect(fechaValida).toBeNull();
    expect(fechaPasada).not.toBeNull();
    expect(horaValida).toBeNull();
  });

  // US16 - Gestionar eventos publicados
  it("testEvent_CalculaCuposRestantes", () => {
    //Arrange
    const evento = new Event({ workersNeeded: 3, acceptedCount: 1, status: EventStatus.OPEN });

    //Act
    const cupos = evento.spotsRemaining();

    //Assert
    expect(cupos).toBe(2);
  });

  // US16 - Gestionar eventos publicados
  it("testEvent_IdentificaEstadosFinales", () => {
    //Arrange
    const completado = new Event({ status: EventStatus.COMPLETED, workersNeeded: 1, acceptedCount: 1 });
    const cancelado = new Event({ status: EventStatus.CANCELLED, workersNeeded: 1, acceptedCount: 0 });

    //Act
    const esCompletado = completado.isCompleted();
    const esCancelado = cancelado.isCancelled();

    //Assert
    expect(esCompletado).toBe(true);
    expect(esCancelado).toBe(true);
  });

  // US17 - Gestionar postulaciones recibidas en un evento
  it("testEventApplication_IdentificaPendienteYAceptada", () => {
    //Arrange
    const pendiente = new EventApplication({ status: ApplicationStatus.PENDING });
    const aceptada = new EventApplication({ status: ApplicationStatus.ACCEPTED });

    //Act
    const esPendiente = pendiente.isPending();
    const esAceptada = aceptada.isAccepted();

    //Assert
    expect(esPendiente).toBe(true);
    expect(esAceptada).toBe(true);
  });

  // US17 - Gestionar postulaciones recibidas en un evento
  it("testEventApplication_IdentificaRechazada", () => {
    //Arrange
    const postulacion = new EventApplication({ status: ApplicationStatus.REJECTED });

    //Act
    const rechazada = postulacion.isRejected();

    //Assert
    expect(rechazada).toBe(true);
    expect(postulacion.isAccepted()).toBe(false);
  });

  // US19 - Postular a un evento
  it("testEvent_FechaLimiteVencida", () => {
    //Arrange
    const evento = new Event({ applicationDeadline: "2000-01-01T00:00:00Z", status: EventStatus.OPEN, workersNeeded: 1, acceptedCount: 0 });

    //Act
    const vencida = evento.deadlinePassed();

    //Assert
    expect(vencida).toBe(true);
  });

  // US19 - Postular a un evento
  it("testEventApplication_IdentificaRetirada", () => {
    //Arrange
    const postulacion = new EventApplication({ status: ApplicationStatus.WITHDRAWN });

    //Act
    const retirada = postulacion.isWithdrawn();

    //Assert
    expect(retirada).toBe(true);
  });
});
