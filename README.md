# AddressManagement

RESTful Address-Management-API mit ASP.NET Core und .NET 10.

## Funktionsumfang

Die API verwaltet Adressen mit den Pflichtfeldern `FirstName`, `LastName`, `Street`, `HouseNumber`, `PostalCode`, `City` und `Country` 
sowie der optionalen E-Mail-Adresse `Email`. 
Jede Adresse besitzt eine GUID als `Id`.

| Methode | Endpunkt | Funktion | Erfolgsstatus |
| --- | --- | --- | --- |
| POST | `/api/addresses` | Adresse anlegen, inklusive `Location`-Header | `201 Created` |
| GET | `/api/addresses/{id}` | Adresse anhand ihrer ID abrufen | `200 OK` |
| GET | `/api/addresses` | Adressen auflisten; optional nach `city` und `postalCode` filtern | `200 OK` |
| PUT | `/api/addresses/{id}` | Adresse vollständig aktualisieren | `204 No Content` |
| DELETE | `/api/addresses/{id}` | Adresse löschen | `204 No Content` |

Für unbekannte IDs wird `404 Not Found` zurückgegeben. 
Ungültige Eingaben werden mit `400 Bad Request` abgelehnt. 
Fehlerantworten verwenden *Problem Details* (RFC 7807).

## Architektur

Die Lösung trennt Verantwortlichkeiten in vier Schichten:

- **AddressManagement.Api:** HTTP-Endpunkte, Statuscodes und Modelvalidierung.
- **AddressManagement.Application:** `AddressService` mit Anwendungslogik sowie `IAddressRepository` als Abstraktion für den Datenzugriff.
- **AddressManagement.Domain:** Entität `Address` mit Validierungsattributen.
- **AddressManagement.Infrastructure:** EF-Core-`DbContext` und Implementierung des Repositorys.

Der Controller verwendet den Service; der Service greift über `IAddressRepository` auf die Persistenz zu. Die konkreten Implementierungen werden über Dependency Injection eingebunden.

## Designentscheidungen

- **Service-Repository-Pattern:** Trennung von HTTP-Verarbeitung, Anwendungslogik und Datenzugriff.
- **EF Core InMemory:** Entspricht der Aufgabenstellung; keine externe Datenbank oder zusätzliche Infrastruktur notwendig. Daten sind nicht dauerhaft gespeichert.
- **Data Annotations:** Pflichtfelder werden mit `[Required]`, das optionale E-Mail-Feld bei Angabe mit `[EmailAddress]` validiert.
- **REST-konforme Antworten:** `201` mit `Location` bei Erstellung, `204` bei erfolgreicher Änderung/Löschung sowie Problem Details für Fehlerfälle.
- **Test-Driven Development:** Entwicklung in RED-/GREEN-Schritten mit getrennten Unit- und Integrationstests.

## Voraussetzungen und Ausführung

- .NET 10 SDK

Repository klonen und im Stammverzeichnis der Lösung ausführen:

```bash
dotnet restore
dotnet build
dotnet run --project AddressManagement.Api
```

Die tatsächlich verwendete lokale URL wird beim Start in der Konsole ausgegeben.

Alle Tests ausführen:

```bash
dotnet test
```

Die Lösung enthält Unit-Tests für den Service-Layer und Integrationstests für Repository und HTTP-API. 
Die HTTP-Tests verwenden `WebApplicationFactory`.

## Mögliche Erweiterungen (optional)

Die folgenden Punkte sind **Bonusanforderungen der Aufgabenstellung** und nicht Bestandteil des derzeit beschriebenen Funktionsumfangs:

- [ ] PATCH für partielle Aktualisierungen
- [ ] Optimistic Concurrency (RowVersion)
- [ ] Soft Delete
- [ ] Pagination und Sortierung
- [ ] DTOs mit Mapping statt direkter Verwendung der Entität im API-Vertrag
- [ ] Swagger/OpenAPI-Dokumentation ausbauen
- [ ] Strukturiertes Logging
- [ ] Caching-Konzept
- [ ] JWT-basierte Authentifizierung und Autorisierung

Diese Liste dokumentiert mögliche Erweiterungen, ohne sie als bereits implementiert auszuweisen.
