<a href="https://github.com/ciennce/Sicherheitsfreigabe/commit/0dbe541ec41f002a95f8ab7b0906adf44bb621ea">Claude usage:<a>

## [![Repography logo](https://images.repography.com/logo.svg)](https://repography.com) / Recent activity [![Time period](https://images.repography.com/159967951/ciennce/Sicherheitsfreigabe/recent-activity/TSNXUjia3BXS8LN7uyAiHNQySTopZ9CZiAcRgM9_FWA/yUM1eJXmkgO7vzdMwOdXWtgQkaeoJCh1u342uxA_JJ4_badge.svg)](https://repography.com)
[![Timeline graph](https://images.repography.com/159967951/ciennce/Sicherheitsfreigabe/recent-activity/TSNXUjia3BXS8LN7uyAiHNQySTopZ9CZiAcRgM9_FWA/yUM1eJXmkgO7vzdMwOdXWtgQkaeoJCh1u342uxA_JJ4_timeline.svg)](https://github.com/ciennce/Sicherheitsfreigabe/commits)
[![Top contributors](https://images.repography.com/159967951/ciennce/Sicherheitsfreigabe/recent-activity/TSNXUjia3BXS8LN7uyAiHNQySTopZ9CZiAcRgM9_FWA/yUM1eJXmkgO7vzdMwOdXWtgQkaeoJCh1u342uxA_JJ4_users.svg)](https://github.com/ciennce/Sicherheitsfreigabe/graphs/contributors)
# Fortschrittsbewertung – Sicherheitsfreigabe

Stand: 2026-10-08 (nach Commit `2f7b808` „refractored some stuff“). Abgleich von Code, Klassendiagramm (`Infos/Sicherheitsfreigabe-Klassendiagramm.drawio`) und Vorgehensweise.

## Kurzfazit

Der Syntaxfehler in `Employee.cs` ist behoben und es gibt jetzt eine `Program.cs` mit `Main` – damit sollte sich das Projekt **wieder bauen lassen** (nur noch Warnungen, keine Fehler). `Terminal.CanAccess` und `isAvailable` nutzen jetzt `switch`-Ausdrücke. **Aber:** `Main` ist noch leer, das Programm tut also nichts. Jede Karte ist weiterhin automatisch `green` (Freigabestufe wird nie gesetzt), und durch die Umstellung von `isAvailable` gilt jetzt jemand, der **gleichzeitig** im Urlaub und auf Dienstreise ist, als verfügbar.

## Abgleich mit der 10-Schritte-Vorgehensweise

| # | Schritt | Status |
|---|---------|--------|
| 1 | Modell entwerfen | ⚠️ `Sector` – Rolle im Modell noch unklar |
| 2 | Datenklassen (Mitarbeiter, Sicherheitskarte) | ⚠️ Karte ohne Freigabestufe, `Sector` im Mitarbeiter geht verloren |
| 3 | Datenbankklassen | ✅ funktionsfähig (kleine Aufräumpunkte) |
| 4 | Seed-Daten | ❌ fehlt |
| 5 | Terminal | ⚠️ ohne Mitarbeiter-Prüfung, Signatur weicht vom Diagramm ab |
| 6 | Steuereinheit | ⚠️ `ControlSection` vorhanden, `Challange` noch leer |
| 7 | Main/Programmablauf | ⚠️ `Program.cs` angelegt, `Main` noch leer |
| 8 | Tests | ❌ noch nicht möglich |
| 9 | Protokollsystem (optional) | ❌ offen (ok, optional) |
| 10 | Abgabe | – |

## Was verbessert wurde ✅

Seit dem letzten Stand:

- **Syntaxfehler behoben**: Fehlende `}` bei `IsOnBusinessTrip` in `Model/Employee.cs:15` ergänzt.
- **`Program.cs` mit `Main` angelegt** → Einstiegspunkt vorhanden, Fehler `CS5001` ist weg.
- **`Terminal.CanAccess` als `switch`-Ausdruck** – kürzer und lesbarer als vorher.
- **`ExpandoObject` / `dynamic` aus `ControlSection` entfernt**, ebenso `using System.Numerics`.
- **`GetCardReleaselevel` / `GetCardSector`** übersichtlicher umgebrochen.

Davor schon erledigt:

- `GetCardReleaselevel` gibt die echte Stufe der Karte zurück (statt immer `none`).
- `Employee` ist nicht mehr `abstract` und liegt im Namespace `Sicherheitsfreigabe.Model`.
- Enum `ReleaseLevel` und Klasse `EmployeeData` in PascalCase umbenannt.
- `EmployeeData.GetEmployee` gibt `Employee?` statt `string` zurück.
- Redundante `releaseLevels`-Liste entfernt.
- `CanAccess` ruft `Open()`/`Deny()` nicht mehr selbst auf und gewährt nicht mehr pauschal Zugriff.
- Interface `ITerminal` eingeführt, `ControlSection` angelegt.
- Unidirektionale Beziehung Karte → Mitarbeiter (`Safetycard` kennt nur `ownerId`).

## Noch zu fixen

### 🔴 Wichtig (Logik falsch / Programm tut nichts)

1. **`Safetycard` bekommt keine Freigabestufe** (`Safetycard.cs:15-21`): `releaseLevel` wird nirgends gesetzt, `AddReleaseLevel()` ist leer. Ein nicht gesetztes Enum ist `0` → `green`. **Jede Karte ist `green`**, `CanAccess` gibt also immer `false` zurück.
   → Freigabestufe wieder als Parameter in den Konstruktor aufnehmen.

2. **`Main` ist leer** (`Program.cs:5-8`): Hier müssen Datenbanken und Seed-Daten angelegt und für jede Karte `Challenge` aufgerufen werden.

3. **`ControlSection.Challange` tut nichts** (`ControlSection.cs:13-16`): Das Ergebnis von `GetCard(id)` wird verworfen. `Challenge(kartenId)` soll den gesamten Ablauf steuern:
   1. Karte nachschlagen (`GetCard`) – unbekannte Karte → `Deny()`
   2. Besitzer über `ownerId` in `EmployeeData` holen
   3. `terminal.CanAccess(karte, mitarbeiter)` fragen
   4. `Open()` bzw. `Deny()` auslösen
   5. `lastUsed` der Karte aktualisieren

   Aktuell ist das auf `Challange` (leer) und `Control` (nur Terminal) aufgeteilt – zu **einer** Methode `Challenge` zusammenführen.

4. **`isAvailable` – Reihenfolge im `switch`** (`Model/Employee.cs:27-35`): Der erste passende Fall gewinnt. Weil `{ IsOnBusinessTrip: true } => true` **vor** dem Urlaubs-Fall steht, gilt jemand, der im Urlaub **und** auf Dienstreise eingetragen ist, als verfügbar. Außerdem ist Dienstreise weiterhin „verfügbar“ – wer auf Dienstreise ist, ist aber nicht vor Ort. Vorschlag:
   ```csharp
   public bool IsAvailable() => !IsOnVacation && !IsOnBusinessTrip;
   ```
   Relevant für die Testfälle „rote Karte, Urlaub“ und „rote Karte, Dienstreise“. Die Methode wird außerdem noch nirgends aufgerufen.

### 🟠 Fehlende Teile für den Ablauf

5. **`Safetycard` fehlen Zugriffsmethoden**: Kein Getter für `ownerId` (Steuereinheit kann den Besitzer nicht ermitteln) und keine Möglichkeit, `lastUsed` zu aktualisieren. Ergänzen z.B. `GetOwnerId()` und `UpdateLastUsed(DateTime)`.

6. **`Terminal.CanAccess` bekommt keinen Mitarbeiter** (`ITerminal.cs:5`, `Terminal.cs:5`): Signatur ist `CanAccess(int id)`, das Diagramm verlangt `CanAccess(karte, mitarbeiter)`. Ohne Mitarbeiter keine Urlaub/Dienstreise-Prüfung. Außerdem holt sich das Terminal die Karte selbst aus der Datenbank – das sollte die Steuereinheit erledigen.

7. **Terminal hat keine eigene Stufe**: Fest verdrahtet ist „nur `red` darf rein“. Für mehrere Türen sollte das Terminal eine Mindeststufe kennen (z.B. Konstruktor-Parameter) und `karte.GetReleaseLevel() >= benoetigteStufe` prüfen.

8. **`Employee` speichert den `Sector` nicht** (`Model/Employee.cs:17-25`): `pSector` wird übergeben, aber es gibt kein Feld dafür – der Wert geht verloren.

9. **Seed-Daten fehlen**: Mindestens 10 Mitarbeiter mit Karten, davon 4 mit Stufe „rot“ (1 Urlaub, 1 Dienstreise, 2 anwesend).

### 🟡 Abweichungen vom Diagramm / Code-Qualität

10. **Enum-Werte** (`ReleaseLevel.cs`): Diagramm `Gruen, Gelb, Rot`, Code `green, blue, red, none` – „Gelb“ ist immer noch `blue`.
11. **`none` an erster Stelle** in `ReleaseLevel` und `Sector` wäre sicherer – dann ist ein vergessener Wert `none` statt `green`/`HR` (siehe Punkt 1).
12. **Unbenutztes `using System.Dynamic;`** in `ControlSection.cs:1` – nach dem Entfernen von `ExpandoObject` nicht mehr nötig.
13. **Unbenutzter Parameter** `release` in `SafeteycardData.Add(Safetycard card, ReleaseLevel release)` – entfernen.
14. **`Employee.GetById` überflüssig**: Das Dictionary liefert schon den richtigen Mitarbeiter, `GetEmployee` kann direkt `employee` zurückgeben.
15. **`Terminal.CanAccess`**: Die Zwischenvariable `result` ist unnötig, man kann den `switch` direkt zurückgeben (`return ... switch { ... };`).
16. **Uneinheitlicher Lebenszyklus**: `SafeteycardData` ist `static`, `EmployeeData` eine normale Klasse. Einheitlich entscheiden.
17. **Benennung**: `SafeteycardData` (Tippfehler) in `Safecard.Data.cs`; `EmployeeData` in `Employee.data.cs`; `Challange` statt `Challenge`; `isAvailable` statt `IsAvailable`; `GetSafetycard()` liefert die Karten-ID → `GetCardId()`. Innerhalb von `SafeteycardData` reicht `GetCard(Id)` statt `SafeteycardData.GetCard(Id)`.
18. **Diagramm aktualisieren**: `HiredDate`/`Birthday`, `Sector`, `ITerminal` und `none` sind im Code, aber nicht im Klassendiagramm.

## Zum Verstehen 💡

- **`switch`-Ausdruck mit Property-Pattern** (in `isAvailable`):
  ```csharp
  return employee switch
  {
      { IsOnBusinessTrip: true } => true,
      { IsOnVacation: true }     => false,
      _                          => true
  };
  ```
  `{ IsOnVacation: true }` heißt „passt, wenn die Eigenschaft `IsOnVacation` `true` ist“. Die Fälle werden **von oben nach unten** geprüft, der **erste** Treffer gewinnt, `_` ist der Rest. Deshalb ist die Reihenfolge entscheidend (siehe Punkt 4).

- **Enum-Standardwert**: Ein Enum-Feld, das nie gesetzt wird, hat den Wert `0` – also den **ersten** Eintrag. Bei `ReleaseLevel` ist das `green`. Deshalb fällt Punkt 1 nicht als Fehler auf: Es gibt keine Meldung, nur stillschweigend falsches Verhalten.

- **`?.` und `??`** (in `GetCardReleaselevel`):
  ```csharp
  return GetCard(Id)?.GetReleaseLevel() ?? ReleaseLevel.none;
  ```
  `?.` ruft `GetReleaseLevel()` nur auf, wenn die Karte nicht `null` ist. `??` nimmt den rechten Wert, wenn links `null` herauskommt. Zusammen: „Stufe der Karte – oder `none`, falls es die Karte nicht gibt“.

- **Sector vs. ReleaseLevel**: Der Plan in `AddReleaseLevel()` ist, die Stufe aus dem Bereich abzuleiten („HR → rot“). Gefordert sind aber 4 rote Karten mit **unterschiedlichen** Situationen (Urlaub, Dienstreise, 2× anwesend) – die Stufe ist also eine Eigenschaft der **Karte**, nicht des Bereichs. Außerdem steht `Sector` doppelt in `Employee` *und* `Safetycard`. Einfachste Lösung: Stufe direkt im Konstruktor der Karte übergeben, `Sector` nur beim Mitarbeiter (oder ganz weglassen, da nicht gefordert).

- **Wer macht was (Single Responsibility)**:
  - `SafeteycardData` / `EmployeeData` → speichern und finden
  - `Terminal.CanAccess(karte, mitarbeiter)` → **nur prüfen**, gibt `true`/`false` zurück
  - `Terminal.Open()` / `Deny()` → Tür
  - `ControlSection.Challenge(kartenId)` → **steuert** den Ablauf und ruft alles der Reihe nach auf
  - `Program.Main` → legt Daten an und ruft für jede Karte `Challenge` auf

- **`static` vs. Instanz**: `SafeteycardData` ist `static` → es gibt genau eine, Aufruf über den Klassennamen. `EmployeeData` muss man mit `new EmployeeData()` erzeugen und dann an `ControlSection` weitergeben (z.B. über den Konstruktor). Beides geht – einheitlich ist aber leichter zu verstehen.

## Empfohlene Reihenfolge

1. Freigabestufe in den `Safetycard`-Konstruktor (Punkt 1).
2. `isAvailable` korrigieren (Punkt 4).
3. `Safetycard` um `GetOwnerId()` und `UpdateLastUsed()` ergänzen (Punkt 5).
4. `CanAccess(Safetycard, Employee)` mit Stufen- und Verfügbarkeitsprüfung bauen (Punkte 6, 7).
5. `ControlSection.Challenge(kartenId)` als kompletten Ablauf implementieren (Punkt 3).
6. `Main` mit Seed-Daten und Schleife über alle Karten füllen (Punkte 2, 9).
7. Testfälle aus Schritt 8 durchspielen und schriftlich festhalten.
8. Aufräumen (Punkte 10–18), optional Protokollsystem.
