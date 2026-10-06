

## [![Repography logo](https://images.repography.com/logo.svg)](https://repography.com) / Recent activity [![Time period](https://images.repography.com/159967951/ciennce/Sicherheitsfreigabe/recent-activity/TSNXUjia3BXS8LN7uyAiHNQySTopZ9CZiAcRgM9_FWA/yUM1eJXmkgO7vzdMwOdXWtgQkaeoJCh1u342uxA_JJ4_badge.svg)](https://repography.com)
[![Timeline graph](https://images.repography.com/159967951/ciennce/Sicherheitsfreigabe/recent-activity/TSNXUjia3BXS8LN7uyAiHNQySTopZ9CZiAcRgM9_FWA/yUM1eJXmkgO7vzdMwOdXWtgQkaeoJCh1u342uxA_JJ4_timeline.svg)](https://github.com/ciennce/Sicherheitsfreigabe/commits)
[![Top contributors](https://images.repography.com/159967951/ciennce/Sicherheitsfreigabe/recent-activity/TSNXUjia3BXS8LN7uyAiHNQySTopZ9CZiAcRgM9_FWA/yUM1eJXmkgO7vzdMwOdXWtgQkaeoJCh1u342uxA_JJ4_users.svg)](https://github.com/ciennce/Sicherheitsfreigabe/graphs/contributors)
# Fortschrittsbewertung – Sicherheitsfreigabe

Stand: 2026-10-06 (nach Commit `8a07b0c` „fixed CardRelesaseLevel Bug“). Abgleich von Code, Klassendiagramm (`Infos/Sicherheitsfreigabe-Klassendiagramm.drawio`) und Vorgehensweise.

## Kurzfazit

Seit dem letzten Stand hat sich einiges getan: `GetEmployee` liefert jetzt ein `Employee`, die redundante `releaseLevels`-Liste ist weg, `CanAccess` ruft `Open()`/`Deny()` nicht mehr selbst auf und es gibt eine erste `ControlSection` (Steuereinheit). **Aber:** Der `CardReleaselevel`-Fix ist noch nicht wirksam (gibt immer `none` zurück), `Employee` ist jetzt `abstract` ohne Unterklassen (kann also nicht instanziiert werden), und **das Projekt kompiliert weiterhin nicht**, weil `Program.cs` / `Main` fehlt.

## Abgleich mit der 10-Schritte-Vorgehensweise

| # | Schritt | Status |
|---|---------|--------|
| 1 | Modell entwerfen | ⚠️ Enum-Werte weichen vom Diagramm ab |
| 2 | Datenklassen (Mitarbeiter, Sicherheitskarte) | ⚠️ `Employee` abstrakt ohne Unterklassen, `Safetycard` fehlen Getter/Setter |
| 3 | Datenbankklassen | ⚠️ `CardReleaselevel` weiterhin kaputt |
| 4 | Seed-Daten | ❌ fehlt |
| 5 | Terminal | ⚠️ ohne Mitarbeiter-Prüfung, Signatur weicht vom Diagramm ab |
| 6 | Steuereinheit | ⚠️ angefangen (`ControlSection`), Ablauf unvollständig |
| 7 | Main/Programmablauf | ❌ fehlt – Projekt baut nicht |
| 8 | Tests | ❌ noch nicht möglich |
| 9 | Protokollsystem (optional) | ❌ offen (ok, optional) |
| 10 | Abgabe | – |

## Bereits erledigt ✅

- `Employeedata.GetEmployee` gibt `Employee?` statt `string` zurück.
- Redundante `releaseLevels`-Liste in `SafeteycardData` entfernt.
- `Terminal.CanAccess` löst `Open()`/`Deny()` nicht mehr selbst aus (Single Responsibility).
- `CanAccess` gewährt nicht mehr pauschal für alle Stufen Zugriff.
- Unbenutztes `using System.Runtime.InteropServices;` aus `Terminal.cs` entfernt.
- Interface `ITerminal` eingeführt.
- Steuereinheit als `ControlSection` angelegt.
- Unidirektionale Beziehung Karte → Mitarbeiter (`Safetycard` kennt nur `ownerId`).

## Noch zu fixen

### 🔴 Blocker (Projekt baut nicht / Logik falsch)

1. **`Program.cs` / `Main` fehlt** – `Sicherheitsfreigabe.csproj` hat `<OutputType>Exe</OutputType>`, ohne Einstiegspunkt gibt es Fehler `CS5001`.

2. **`CardReleaselevel` gibt immer `none` zurück** (`Safecard.Data.cs:25-30`):
   ```csharp
   GetCard(Id)?.GetReleaseLevel();   // Ergebnis wird verworfen
   return releaseLevel.none;         // immer none
   ```
   Folge: `Terminal.CanAccess` landet nie in einem `case`, gibt immer „There currently is no safety level to this card.“ aus und verweigert **jeden** Zugriff. Fix:
   ```csharp
   return GetCard(Id)?.GetReleaseLevel() ?? releaseLevel.none;
   ```

3. **`Employee` ist `abstract`, aber es gibt keine Unterklassen** (`Employee.cs:3`). Damit kann kein einziger Mitarbeiter erzeugt werden → Seed-Daten unmöglich. Entweder `abstract` entfernen oder konkrete Unterklassen anlegen (vermutlich über das neue `Sector`-Enum gedacht, z.B. `HrEmployee`, `LogisticEmployee` …). Wenn Vererbung nicht wirklich gebraucht wird: einfacher ein Feld `Sector` in `Employee`.

4. **`ControlSection.Challange` tut nichts** (`ControlSection.cs:13-16`): Das Ergebnis von `GetCard(id)` wird verworfen. Laut Diagramm soll `Challenge(kartenId)` den gesamten Ablauf steuern:
   1. Karte nachschlagen (`HasCard` / `GetCard`) – unbekannte Karte → `Deny()`
   2. Besitzer über `ownerId` in `Employeedata` holen
   3. `terminal.CanAccess(karte, mitarbeiter)` fragen
   4. `Open()` bzw. `Deny()` auslösen
   5. `lastUsed` der Karte aktualisieren

   Aktuell ist das auf `Challange` (leer) und `Control` (nur Terminal) aufgeteilt – zu **einer** Methode `Challenge` zusammenführen.

### 🟠 Fehlende Teile für den Ablauf

5. **`Safetycard` fehlen Zugriffsmethoden** (`Safetycard.cs`): Es gibt keinen Getter für `ownerId` (Steuereinheit kann den Besitzer nicht ermitteln) und keine Möglichkeit, `lastUsed` zu aktualisieren. Ergänzen z.B. `GetOwnerId()` und `UpdateLastUsed(DateTime)`.

6. **`Terminal.CanAccess` bekommt keinen Mitarbeiter** (`ITerminal.cs:5`, `Terminal.cs:5`): Signatur ist `CanAccess(int id)`, das Diagramm verlangt `CanAccess(karte, mitarbeiter)`. Ohne Mitarbeiter ist die Plausibilitätsprüfung (Urlaub/Dienstreise) nicht möglich. Außerdem greift das Terminal dadurch selbst auf die Datenbank zu – das sollte die Steuereinheit erledigen und dem Terminal nur Karte + Mitarbeiter übergeben.

7. **Terminal hat keine eigene Stufe**: Fest verdrahtet ist „nur `red` darf rein“. Für mehrere Türen/Bereiche sollte das Terminal eine benötigte Mindeststufe kennen (z.B. Konstruktor-Parameter) und `karte.GetReleaseLevel() >= benoetigteStufe` prüfen.

8. **`Employee.isAvailable` – Dienstreise-Zweig ohne Effekt** (`Employee.cs:27-32`): `if (IsOnBusinessTrip) return true;` ist identisch mit dem folgenden `return true;`. Bewusst entscheiden: Wer auf Dienstreise ist, ist physisch nicht vor Ort → vermutlich `return false`. Relevant für Testfall „rote Karte, Dienstreise“. Außerdem: `static` mit Parameter ist unnötig – besser Instanzmethode `IsAvailable()` (wie im Diagramm `IstVerfuegbar()`).

9. **Seed-Daten fehlen**: Mindestens 10 Mitarbeiter mit Karten, davon 4 mit Stufe „rot“ (1 Urlaub, 1 Dienstreise, 2 anwesend).

### 🟡 Abweichungen vom Diagramm / Code-Qualität

10. **Enum-Werte** (`ReleaseLevel.cs`): Diagramm `Gruen, Gelb, Rot`, Code `green, blue, red, none` – „Gelb“ ist immer noch `blue`. Auf `green, yellow, red` (oder deutsch) angleichen. `none` ist neu und nicht im Diagramm → entweder ins Diagramm aufnehmen oder stattdessen mit `releaseLevel?` arbeiten.
11. **Enum-Name `releaseLevel`** ist lowerCamelCase → in C# üblich `ReleaseLevel`.
12. **Unbenutzter Parameter** `release` in `SafeteycardData.Add(Safetycard card, releaseLevel release)` – das Level steckt schon in der Karte, Parameter entfernen.
13. **`Employee.GetById` überflüssig** (`Employee.cs:41-45`): Das Dictionary in `Employeedata` liefert bereits den richtigen Mitarbeiter, `GetEmployee` kann direkt `employee` zurückgeben.
14. **Unbenutzte `using`s**: `System.ComponentModel.Design` (`Safecard.Data.cs:1`), `System.Numerics` (`ControlSection.cs:1`).
15. **Uneinheitlicher Lebenszyklus**: `SafeteycardData` ist `static`, `Employeedata` eine normale Klasse. Einheitlich entscheiden (z.B. beide als Instanzen in `Main` erzeugen und an `ControlSection` übergeben).
16. **Benennung**: Klasse `SafeteycardData` (Tippfehler) in Datei `Safecard.Data.cs`; `Employeedata` in `Employee.data.cs`; Methode `Challange` statt `Challenge`; `GetSafetycard()` liefert eigentlich die Karten-ID → `GetCardId()`. Einheitlich z.B. `SafetycardData.cs` / `EmployeeData.cs`.
17. **`Sector`-Enum** ist neu, wird aber nirgends verwendet; ist weder im Diagramm noch in der Vorgehensweise vorgesehen. Entweder einbauen (siehe Punkt 3) oder entfernen.
18. **Diagramm aktualisieren**: Felder `HiredDate`/`Birthday`, `Sector`, `ITerminal` und `none` sind im Code, aber nicht im Klassendiagramm.

## Empfohlene Reihenfolge

1. `CardReleaselevel` fixen (Punkt 2).
2. `abstract` bei `Employee` entfernen oder Unterklassen anlegen (Punkt 3).
3. `Safetycard` um `GetOwnerId()` und `UpdateLastUsed()` ergänzen (Punkt 5).
4. `CanAccess(Safetycard, Employee)` mit Stufen- und Verfügbarkeitsprüfung bauen (Punkte 6–8).
5. `ControlSection.Challenge(kartenId)` als kompletten Ablauf implementieren (Punkt 4).
6. `Program.cs` mit Seed-Daten und Schleife über alle Karten schreiben (Punkte 1, 9).
7. Testfälle aus Schritt 8 durchspielen und schriftlich festhalten.
8. Aufräumen (Punkte 10–18), optional Protokollsystem.
