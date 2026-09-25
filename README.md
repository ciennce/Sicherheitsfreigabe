
## [![Repography logo](https://images.repography.com/logo.svg)](https://repography.com) / Recent activity [![Time period](https://images.repography.com/159967951/ciennce/Sicherheitsfreigabe/recent-activity/TSNXUjia3BXS8LN7uyAiHNQySTopZ9CZiAcRgM9_FWA/yUM1eJXmkgO7vzdMwOdXWtgQkaeoJCh1u342uxA_JJ4_badge.svg)](https://repography.com)
[![Timeline graph](https://images.repography.com/159967951/ciennce/Sicherheitsfreigabe/recent-activity/TSNXUjia3BXS8LN7uyAiHNQySTopZ9CZiAcRgM9_FWA/yUM1eJXmkgO7vzdMwOdXWtgQkaeoJCh1u342uxA_JJ4_timeline.svg)](https://github.com/ciennce/Sicherheitsfreigabe/commits)
[![Top contributors](https://images.repography.com/159967951/ciennce/Sicherheitsfreigabe/recent-activity/TSNXUjia3BXS8LN7uyAiHNQySTopZ9CZiAcRgM9_FWA/yUM1eJXmkgO7vzdMwOdXWtgQkaeoJCh1u342uxA_JJ4_users.svg)](https://github.com/ciennce/Sicherheitsfreigabe/graphs/contributors)

# Fortschrittsbewertung – Sicherheitsfreigabe

Stand: 2026-09-25, Abgleich von Code, Klassendiagramm (`Infos/Sicherheitsfreigabe-Klassendiagramm.drawio`) und `Vorgehensweise-Sicherheitsfreigabe.txt`.

## Kurzfazit

Die Datenmodell-Ebene (Schritte 1–3 der Vorgehensweise) steht größtenteils, hat aber zwei konkrete Bugs. Alles ab Schritt 4 (Seed-Daten, Terminal-Logik, Steuereinheit, Main, Tests) fehlt noch bzw. ist unvollständig. **Das Projekt kompiliert aktuell nicht** – `OutputType` ist `Exe`, aber es gibt keine `Program.cs` / keinen `Main`.

## Abgleich mit der 10-Schritte-Vorgehensweise

| # | Schritt | Status |
|---|---------|--------|
| 1 | Modell entwerfen | ⚠️ teilweise |
| 2 | Datenklassen (Mitarbeiter, Sicherheitskarte) | ⚠️ weitgehend, ein Logik-Punkt fragwürdig |
| 3 | Datenbankklassen | ⚠️ vorhanden, aber mit Bug |
| 4 | Seed-Daten | ❌ fehlt |
| 5 | Terminal | ⚠️ verstößt gegen eigene Vorgabe |
| 6 | Steuereinheit | ❌ fehlt komplett |
| 7 | Main/Programmablauf | ❌ fehlt komplett |
| 8 | Tests | ❌ noch nicht möglich |
| 9 | Protokollsystem (optional) | ❌ noch offen (ok, optional) |
| 10 | Abgabe | – |

## Details

### 1. Modell
- Unidirektionale Beziehung Karte → Mitarbeiter ist korrekt umgesetzt: `Safetycard` speichert nur `ownerId` (int), `Employee` kennt keine Karte. ✅
- Freigabestufe als Enum umgesetzt (`ReleaseLevel.cs`). ✅
- **Abweichung vom eigenen Diagramm:** Das Klassendiagramm sieht die Werte `Gruen, Gelb, Rot` vor, im Code steht `green, blue, red` (`ReleaseLevel.cs:5-7`) – „Gelb" wurde zu „blue". Vermutlich ein Versehen beim Tippen.
- `Steuereinheit` ist im Diagramm vorhanden, im Code nicht.

### 2. Datenklassen
- `Employee`: Felder passen (Id, Name, HiredDate, Birthday, IsOnVacation, IsOnBusinessTrip).
- Fragwürdig: `Employee.isAvailable()` (`Employee.cs:27-32`):
  ```csharp
  if (employee.isOnVacation) return false;
  if (employee.isOnBusinessTrip) return true;
  return true;
  ```
  Die beiden letzten Zeilen sind äquivalent – der `isOnBusinessTrip`-Zweig hat aktuell keinen Effekt. Die Aufgabenstellung nennt „Urlaub/Dienstreise" aber explizit als zwei unterschiedliche Plausibilitätsfälle für Schritt 8 (Testfall „rote Karte, Dienstreise"). Hier sollte überlegt werden, ob Dienstreise wirklich identisch zu „verfügbar" behandelt werden soll oder nicht.
- `Safetycard`: Felder entsprechen exakt der Vorgabe (Karten-ID, Besitzer-Id, letztes Nutzungsdatum, Freigabestufe). ✅
- Namenskonvention: `isAvailable`, `releaseLevel` (Enum) sind lowerCamelCase statt der in C# üblichen PascalCase-Konvention (`IsAvailable`, `ReleaseLevel`). Kein funktionaler Fehler, aber inkonsistent zur restlichen Codebasis (z.B. `GetId`, `GetName` sind PascalCase).

### 3. Datenbankklassen
- `SafeteycardData.HasCard` / `GetCard` sind vorhanden wie gefordert. ✅
- **Bug in `Employeedata.GetEmployee`** (`Employee.data.cs:12-16`): Laut Diagramm soll die Methode `Mitarbeiter` zurückgeben (`GetEmployee(id: int): Mitarbeiter`), aktuell gibt sie nur `employee?.GetName()`, also einen `string`, zurück. Dadurch geht die Information verloren, ob der Mitarbeiter im Urlaub/auf Dienstreise ist – genau die Information, die später für `CanAccess` gebraucht wird.
- **Bug in `SafeteycardData.CardReleaselevel`** (`Safecard.Data.cs:26-30`):
  ```csharp
  int index = releaseLevels.IndexOf((releaseLevel)Id);
  return releaseLevels[index];
  ```
  Hier wird die Karten-`Id` direkt in einen `releaseLevel`-Enum-Wert gecastet und dessen Position in der separaten `releaseLevels`-Liste gesucht. Das hat keinen Bezug zur tatsächlich gespeicherten Karte. Beispiel: Karte mit `Id = 5` würde als `(releaseLevel)5` interpretiert (out of range) und dann in der Liste gesucht – das Ergebnis ist zufällig/falsch. Richtig wäre `GetCard(Id)?.GetReleaseLevel()`.
- Die separate `releaseLevels`-Liste in `SafeteycardData` ist redundant, weil jede `Safetycard` ihr Level selbst kennt (`GetReleaseLevel()`) – sie ist die Ursache des obigen Bugs und kann ersatzlos entfernt werden.
- Zum Lebenszyklus-Gedanken aus der Vorgehensweise (Singleton vs. einmal erzeugte Instanz): kann noch nicht bewertet werden, da es noch kein `Main` gibt, das die Datenbanken instanziiert.

### 4. Seed-Daten
Fehlt komplett – keine Datei/Methode, die die geforderten mindestens 10 Mitarbeiter (davon 4 mit „rot": 1 Urlaub, 1 Dienstreise, 2 anwesend) und die zugehörigen Karten anlegt.

### 5. Terminal
- `Terminal.CanAccess` (`Terminal.cs:10-21`) ruft `Open()`/`Deny()` **selbst** auf. Die Vorgehensweise verlangt aber ausdrücklich, dass `CanAccess` reine Prüf-Logik ist, **ohne** den Türmechanismus selbst auszulösen (Single-Responsibility – das soll die Steuereinheit übernehmen).
- Der `switch` gewährt für **alle drei** Stufen (`green`, `blue`/„gelb", `red`) Zugriff und öffnet immer – es gibt aktuell keinerlei echte Prüfung, ob die Kartenstufe zur Terminal-/Türstufe passt.
- Das Diagramm sieht `CanAccess(karte, mitarbeiter): bool` vor – die aktuelle Signatur `CanAccess(int id, SafeteycardData safetycard)` bekommt gar keinen `Mitarbeiter` übergeben, kann also die geforderte Plausibilitätsprüfung (Urlaub/Dienstreise) gar nicht durchführen.
- Unbenutztes `using System.Runtime.InteropServices;` in `Terminal.cs:1`.

### 6. Steuereinheit
Fehlt vollständig. Es gibt keine Klasse mit `Challenge(kartenId)`, die Karte nachschlägt → Mitarbeiter ermittelt → Terminal fragt → Tür öffnen/verweigern lässt → Nutzungsdatum aktualisiert.

### 7. Main/Programmablauf
Es existiert keine `Program.cs` im Projekt. Da `Sicherheitsfreigabe.csproj` `<OutputType>Exe</OutputType>` setzt, aber kein Einstiegspunkt vorhanden ist, **lässt sich das Projekt aktuell nicht bauen**.

### 8. Tests
Noch nicht sinnvoll möglich, da der Kernablauf (Steuereinheit, Main) fehlt.

### 9. Protokollsystem
Laut Aufgabe optional – bewusst noch nicht begonnen, kein Handlungsbedarf.

## Weitere Beobachtungen (Code-Qualität, keine Blocker)
- Tippfehler/Inkonsistenz: Klasse heißt `SafeteycardData` (Datei `Safecard.Data.cs`), während die zugehörige Datenklasse korrekt `Safetycard` heißt.
- Dateibenennung uneinheitlich: `Employee.data.cs` vs. `Safecard.Data.cs` (unterschiedliche Groß-/Kleinschreibung, unterschiedliches Namensschema).

## Empfohlene nächste Schritte (Priorität)

1. `ReleaseLevel`-Enum ans Diagramm angleichen (`green, blue, red` → `Gruen/Grün, Gelb, Rot` oder konsistent Englisch) – Namenskonsistenz klären.
2. `CardReleaselevel`-Bug fixen: über `GetCard(Id)?.GetReleaseLevel()` statt über den Cast-Trick.
3. `Employeedata.GetEmployee` so ändern, dass es das `Employee`-Objekt zurückgibt (nicht nur den Namen).
4. `CanAccess` neu bauen: Signatur mit Karte *und* Mitarbeiter, echte Prüfung Freigabestufe-passt-zu-Terminal UND Urlaub/Dienstreise-Plausibilität – **ohne** `Open()`/`Deny()` selbst aufzurufen.
5. `Steuereinheit` mit `Challenge(kartenId)` als Orchestrator bauen.
6. `Program.cs` schreiben: Instanzen/Seed-Daten erzeugen (10 Mitarbeiter, 4 rote Karten wie gefordert), Schleife über alle Karten, die jeweils `Challenge` auslösen.
7. Testfälle aus der Vorgehensweise (Schritt 8) schriftlich festhalten und durchspielen.
8. Optional danach: Protokollsystem ergänzen.
