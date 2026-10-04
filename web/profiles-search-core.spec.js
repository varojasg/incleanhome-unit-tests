import { describe, it, expect } from "vitest";
import { ClientProfile } from "../src/Profiles/domain/model/client-profile.entity.js";
import { WorkerProfile } from "../src/Profiles/domain/model/worker-profile.entity.js";
import { SearchFilters } from "../src/SearchAndCatalog/domain/model/search-filters.value.js";
import { WorkerCatalogItem } from "../src/SearchAndCatalog/domain/model/worker-catalog-item.entity.js";

describe("Profiles and search core unit tests", () => {
  // US05 - Gestionar perfil
  it("testClientProfile_MapeaDatos", () => {
    //Arrange
    const raw = { name: "Ana Torres", phone: "987654321", email: "ana@mail.com" };

    //Act
    const perfil = ClientProfile.fromApi(raw);

    //Assert
    expect(perfil.name).toBe("Ana Torres");
    expect(perfil.phone).toBe("987654321");
    expect(perfil.initials()).toBe("AT");
  });

  // US05 - Gestionar perfil
  it("testWorkerProfile_ValoresVacios", () => {
    //Arrange
    const raw = {};

    //Act
    const perfil = WorkerProfile.fromApi(raw);

    //Assert
    expect(perfil.name).toBe("");
    expect(perfil.serviceTypes).toEqual([]);
    expect(perfil.zones).toEqual([]);
  });

  // US06 - Buscar prestadores de servicios
  it("testSearchFilters_SoloEnviaFiltrosConValor", () => {
    //Arrange
    const filtros = new SearchFilters({ serviceType: "cleaning", zone: "", minAge: 25, minRating: null });

    //Act
    const parametros = filtros.toParams();

    //Assert
    expect(parametros).toEqual({ serviceType: "cleaning", minAge: 25 });
  });

  // US06 - Buscar prestadores de servicios
  it("testSearchFilters_FiltrosVacios", () => {
    //Arrange
    const filtros = SearchFilters.empty();

    //Act
    const vacio = filtros.isEmpty();

    //Assert
    expect(vacio).toBe(true);
    expect(filtros.toParams()).toEqual({});
  });

  // US07 - Consultar perfil de prestador
  it("testWorkerCatalogItem_MapeaInformacion", () => {
    //Arrange
    const raw = { id: 5, name: "María López", age: 30, gender: "female", hourlyRate: 35, serviceTypes: ["cleaning"], zones: ["san-isidro"] };

    //Act
    const trabajador = WorkerCatalogItem.fromApi(raw);

    //Assert
    expect(trabajador.name).toBe("María López");
    expect(trabajador.hourlyRate).toBe(35);
    expect(trabajador.serviceTypes).toEqual(["cleaning"]);
  });

  // US07 - Consultar perfil de prestador
  it("testWorkerCatalogItem_GeneraIniciales", () => {
    //Arrange
    const trabajador = new WorkerCatalogItem({ id: 2, name: "María López" });

    //Act
    const iniciales = trabajador.initials();

    //Assert
    expect(iniciales).toBe("ML");
  });
});
