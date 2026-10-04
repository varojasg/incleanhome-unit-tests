import { describe, it, expect } from "vitest";
import { Role, isValidRole } from "../src/IAM/domain/model/role.value.js";
import { User } from "../src/IAM/domain/model/user.entity.js";
import { required, strongPassword, minItems, email, totpCode } from "../src/Shared/domain/validation/validators.js";

describe("IAM core unit tests", () => {
  // US01 - Registro de usuario según rol
  it("testRegistroCliente_DatosObligatorios", () => {
    //Arrange
    const validarNombre = required();
    const validarPassword = strongPassword();

    //Act
    const nombreValido = validarNombre("Ana");
    const passwordValido = validarPassword("Clave123");
    const nombreInvalido = validarNombre("   ");

    //Assert
    expect(nombreValido).toBeNull();
    expect(passwordValido).toBeNull();
    expect(nombreInvalido).not.toBeNull();
  });

  // US01 - Registro de usuario según rol
  it("testRegistroPrestador_RequiereAlMenosUnServicio", () => {
    //Arrange
    const validarServicios = minItems(1);

    //Act
    const sinServicios = validarServicios([]);
    const conServicio = validarServicios(["cleaning"]);

    //Assert
    expect(sinServicios).not.toBeNull();
    expect(conServicio).toBeNull();
  });

  // US02 - Gestionar inicio de sesión
  it("testLogin_CorreoValido", () => {
    //Arrange
    const validarCorreo = email();

    //Act
    const correcto = validarCorreo("usuario@correo.com");
    const incorrecto = validarCorreo("usuario-correo.com");

    //Assert
    expect(correcto).toBeNull();
    expect(incorrecto).not.toBeNull();
  });

  // US02 - Gestionar inicio de sesión
  it("testUsuarioAutenticado_IdentificaRol", () => {
    //Arrange
    const raw = { id: 1, name: "Ana", email: "ana@mail.com", phone: "999999999", role: Role.CLIENT };

    //Act
    const usuario = User.fromApi(raw);

    //Assert
    expect(usuario.isClient()).toBe(true);
    expect(usuario.isWorker()).toBe(false);
    expect(isValidRole(usuario.role)).toBe(true);
  });

  // US04 - Autenticación de dos factores
  it("testCodigo2FA_SeisDigitos", () => {
    //Arrange
    const validarCodigo = totpCode();

    //Act
    const resultado = validarCodigo("123456");

    //Assert
    expect(resultado).toBeNull();
  });

  // US04 - Autenticación de dos factores
  it("testCodigo2FA_FormatoInvalido", () => {
    //Arrange
    const validarCodigo = totpCode();

    //Act
    const corto = validarCodigo("12345");
    const letras = validarCodigo("12A456");

    //Assert
    expect(corto).not.toBeNull();
    expect(letras).not.toBeNull();
  });
});
