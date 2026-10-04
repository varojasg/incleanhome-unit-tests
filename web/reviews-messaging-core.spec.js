import { describe, it, expect } from "vitest";
import { Review } from "../src/ReviewsAndEvaluation/domain/model/review.entity.js";
import { Message } from "../src/Shared/domain/model/message.entity.js";
import { MessagingService } from "../src/Shared/application/messaging.service.js";
import { min, max } from "../src/Shared/domain/validation/validators.js";

describe("Reviews and messaging core unit tests", () => {
  // US13 - Calificar servicio completado
  it("testReview_MapeaCalificacion", () => {
    //Arrange
    const raw = { id: 1, bookingId: 10, workerId: 2, clientId: 1, clientName: "Ana", rating: 5, comment: "Excelente", createdAt: "2026-10-03" };

    //Act
    const review = Review.fromApi(raw);

    //Assert
    expect(review.rating).toBe(5);
    expect(review.comment).toBe("Excelente");
    expect(review.bookingId).toBe(10);
  });

  // US13 - Calificar servicio completado
  it("testReview_CalificacionEntreUnoYCinco", () => {
    //Arrange
    const minimo = min(1);
    const maximo = max(5);

    //Act
    const uno = [minimo(1), maximo(1)];
    const cinco = [minimo(5), maximo(5)];
    const cero = minimo(0);
    const seis = maximo(6);

    //Assert
    expect(uno).toEqual([null, null]);
    expect(cinco).toEqual([null, null]);
    expect(cero).not.toBeNull();
    expect(seis).not.toBeNull();
  });

  // US14 - Gestionar mensajería directa
  it("testMessage_IdentificaMensajesPropios", () => {
    //Arrange
    const mensaje = Message.fromApi({ id: 1, senderId: 10, recipientId: 20, content: "Hola", createdAt: "2026-10-03", readAt: null });

    //Act
    const propioNumero = mensaje.isMine(10);
    const propioTexto = mensaje.isMine("10");

    //Assert
    expect(propioNumero).toBe(true);
    expect(propioTexto).toBe(true);
  });

  // US14 - Gestionar mensajería directa
  it("testSendMessage_RechazaMensajeVacio", async () => {
    //Arrange
    const contenido = "   ";

    //Act
    const accion = MessagingService.sendMessage(20, contenido);

    //Assert
    await expect(accion).rejects.toThrow("El mensaje no puede estar vacío");
  });
});
