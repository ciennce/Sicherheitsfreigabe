<a href="https://github.com/ciennce/Sicherheitsfreigabe/commit/0dbe541ec41f002a95f8ab7b0906adf44bb621ea">Claude usage:<a>

## [![Repography logo](https://images.repography.com/logo.svg)](https://repography.com) / Recent activity [![Time period](https://images.repography.com/159967951/ciennce/Sicherheitsfreigabe/recent-activity/TSNXUjia3BXS8LN7uyAiHNQySTopZ9CZiAcRgM9_FWA/yUM1eJXmkgO7vzdMwOdXWtgQkaeoJCh1u342uxA_JJ4_badge.svg)](https://repography.com)
[![Timeline graph](https://images.repography.com/159967951/ciennce/Sicherheitsfreigabe/recent-activity/TSNXUjia3BXS8LN7uyAiHNQySTopZ9CZiAcRgM9_FWA/yUM1eJXmkgO7vzdMwOdXWtgQkaeoJCh1u342uxA_JJ4_timeline.svg)](https://github.com/ciennce/Sicherheitsfreigabe/commits)
[![Top contributors](https://images.repography.com/159967951/ciennce/Sicherheitsfreigabe/recent-activity/TSNXUjia3BXS8LN7uyAiHNQySTopZ9CZiAcRgM9_FWA/yUM1eJXmkgO7vzdMwOdXWtgQkaeoJCh1u342uxA_JJ4_users.svg)](https://github.com/ciennce/Sicherheitsfreigabe/graphs/contributors)
# Fortschrittsbewertung – Sicherheitsfreigabe

Stand: 2026-10-12 (nach Commit `9189212` „added card.ReleaseLevel();“). Abgleich von Code, Klassendiagramm (`Infos/Sicherheitsfreigabe-Klassendiagramm.drawio`) und Vorgehensweise.

## Kurzfazit

Das Datenmodell steht jetzt: Karten bekommen ihre Freigabestufe automatisch aus dem `Sector`, `IsAvailable()` prüft Urlaub **und** Dienstreise korrekt, und das Projekt sollte sich fehlerfrei bauen lassen. **Was noch fehlt, ist der eigentliche Ablauf:** `Main` ist leer, es gibt keine Seed-Daten, und `Challange` prüft nur die Kartenstufe – der Mitarbeiter (Urlaub/Dienstreise) wird noch nicht berücksichtigt. Eine rote Karte im Urlaub würde aktuell also durchgelassen.

## Abgleich mit der 10-Schritte-Vorgehensweise

| # | Schritt | Status |
|---|---------|--------|
| 1 | Modell entwerfen | ✅ Stufe wird aus `Sector` abgeleitet |
| 2 | Datenklassen (Mitarbeiter, Sicherheitskarte) | ⚠️ `Safetycard` fehlen `GetOwnerId()` / `UpdateLastUsed()` |
| 3 | Datenbankklassen | ✅ funktionsfähig (kleine Aufräumpunkte) |
| 4 | Seed-Daten | ❌ fehlt |
| 5 | Terminal | ⚠️ ohne Mitarbeiter-Prüfung, Signatur weicht vom Diagramm ab |
| 6 | Steuereinheit | ⚠️ `Challange` ruft `Control` auf, prüft aber keinen Mitarbeiter |
| 7 | Main/Programmablauf | ⚠️ `Program.cs` vorhanden, `Main` noch leer |
| 8 | Tests | ❌ noch nicht möglich |
| 9 | Protokollsystem (optional) | ❌ offen (ok, optional) |
| 10 | Abgabe | – |

## Was verbessert wurde ✅

Seit dem letzten Stand:

- **Freigabestufe wird gesetzt**: `Safetycard.AddReleaseLevel()` leitet die Stufe per `switch` aus dem `Sector` ab und wird im Konstruktor **nach** `Sector = sector;` aufgerufen (richtige Reihenfolge). Damit ist nicht mehr jede Karte `green`.
- **`IsAvailable()` korrigiert**: `!IsOnVacation && !IsOnBusinessTrip` – Urlaub und Dienstreise heißen jetzt beide „nicht verfügbar“, als Instanzmethode in PascalCase.
- **`Sector` nur noch an der Karte**: Der ungenutzte `pSector`-Parameter ist aus `Employee` entfernt – keine doppelte Information mehr.
- **`Challange` tut etwas**: ruft jetzt `Control(id)` auf, sodass Tür auf/zu tatsächlich ausgelöst wird.
- **Unbenutztes `using System.Dynamic`** aus `ControlSection` entfernt.

Davor schon erledigt:

- `Program.cs` mit `Main` angelegt (Fehler `CS5001` behoben), Syntaxfehler in `Employee.cs` behoben.
- `Terminal.CanAccess` als `switch`-Ausdruck, `ExpandoObject`/`dynamic` entfernt.
- `GetCardReleaselevel` gibt die echte Stufe zurück (statt immer `none`).
- `Employee` nicht mehr `abstract`, liegt in `Sicherheitsfreigabe.Model`.
- `ReleaseLevel` / `EmployeeData` in PascalCase, `GetEmployee` gibt `Employee?` zurück.
- Redundante `releaseLevels`-Liste entfernt, `CanAccess` ruft `Open()`/`Deny()` nicht mehr selbst auf.
- Interface `ITerminal`, `ControlSection` angelegt, unidirektionale Beziehung Karte → Mitarbeiter.

## Noch zu fixen

### 🔴 Wichtig (Ablauf fehlt / Logik unvollständig)

1. **`Main` ist leer** (`Program.cs:5-8`): Hier müssen Mitarbeiter und Karten angelegt und für jede Karte `Challenge` aufgerufen werden. Skizze:
   ```csharp
   var employees = new EmployeeData();
   employees.Add(new Employee("Anna", 1, false, false, ...));
   SafeteycardData.Add(new Safetycard(100, 1, DateTime.Now, Sector.HR));
   // ... mind. 10 Mitarbeiter + Karten

   var control = new ControlSection(employees);
   foreach (int cardId in new[] { 100, 101, ... })
       control.Challenge(cardId);
   ```

2. **Mitarbeiter wird nicht geprüft** (`ControlSection.cs`, `Terminal.cs`): `IsAvailable()` ist fertig, wird aber nirgends aufgerufen. `Control` fragt nur `CanAccess(id)`, und das schaut nur auf die Kartenstufe. Folge: **Rote Karte + Urlaub/Dienstreise → Zugang gewährt**. Genau das sind die Testfälle aus Schritt 8.

3. **`Challenge` soll den ganzen Ablauf steuern** (`ControlSection.cs:11-25`). Aktuell leitet `Challange` nur an `Control` weiter. Laut Diagramm:
   1. Karte nachschlagen (`GetCard`) – unbekannte Karte → `Deny()`
   2. Besitzer über `ownerId` in `EmployeeData` holen
   3. `terminal.CanAccess(karte, mitarbeiter)` fragen
   4. `Open()` bzw. `Deny()` auslösen
   5. `lastUsed` der Karte aktualisieren

   Dafür braucht `ControlSection` Zugriff auf `EmployeeData` (z.B. per Konstruktor-Parameter). `Challange` und `Control` können dann zu **einer** Methode `Challenge` zusammengeführt werden.

### 🟠 Fehlende Teile für den Ablauf

4. **`Safetycard` fehlen Zugriffsmethoden**: Kein Getter für `ownerId` (Steuereinheit kann den Besitzer nicht ermitteln) und keine Möglichkeit, `lastUsed` zu aktualisieren. Ergänzen z.B. `GetOwnerId()` und `UpdateLastUsed(DateTime)`.

5. **`Terminal.CanAccess` bekommt keinen Mitarbeiter** (`ITerminal.cs:5`, `Terminal.cs:5`): Signatur ist `CanAccess(int id)`, das Diagramm verlangt `CanAccess(karte, mitarbeiter)`. Vorschlag:
   ```csharp
   public bool CanAccess(Safetycard card, Employee employee)
       => card.GetReleaseLevel() == ReleaseLevel.red && employee.IsAvailable();
   ```
   Dann holt sich das Terminal auch nichts mehr selbst aus der Datenbank – das macht die Steuereinheit.

6. **Terminal hat keine eigene Stufe**: Fest verdrahtet ist „nur `red` darf rein“. Für mehrere Türen könnte das Terminal eine Mindeststufe im Konstruktor bekommen und `>=` prüfen. Optional – für die Aufgabe reicht ein rotes Terminal.

7. **Seed-Daten fehlen**: Mindestens 10 Mitarbeiter mit Karten, davon 4 mit Stufe „rot“ (1 Urlaub, 1 Dienstreise, 2 anwesend). Da die Stufe jetzt aus dem `Sector` kommt, heißt das: **4 Karten mit `Sector.HR`**.

### 🟡 Abweichungen vom Diagramm / Code-Qualität

8. **Enum-Werte** (`ReleaseLevel.cs`): Diagramm `Gruen, Gelb, Rot`, Code `green, blue, red, none` – „Gelb“ ist immer noch `blue`.
9. **Unbenutzter Parameter** `release` in `SafeteycardData.Add(Safetycard card, ReleaseLevel release)` – die Stufe kommt jetzt aus der Karte selbst, Parameter entfernen.
10. **`Employee.GetById` überflüssig**: Das Dictionary liefert schon den richtigen Mitarbeiter, `GetEmployee` kann direkt `employee` zurückgeben.
11. **`Terminal.CanAccess`**: Die Zwischenvariable `result` ist unnötig, den `switch` direkt zurückgeben.
12. **Uneinheitlicher Lebenszyklus**: `SafeteycardData` ist `static`, `EmployeeData` eine normale Klasse. Einheitlich entscheiden.
13. **Benennung**: `SafeteycardData` (Tippfehler) in `Safecard.Data.cs`; `EmployeeData` in `Employee.data.cs`; `Challange` statt `Challenge`; `GetSafetycard()` liefert die Karten-ID → `GetCardId()`; Konstruktor-Parameter `sector` statt `pSector` (sonst überall `p…`). Innerhalb von `SafeteycardData` reicht `GetCard(Id)` statt `SafeteycardData.GetCard(Id)`.
14. **Diagramm aktualisieren**: `HiredDate`/`Birthday`, `Sector` (an der Karte), `ITerminal`, `none` und die Ableitung Sector → Freigabestufe sind im Code, aber nicht im Klassendiagramm.

## Zum Verstehen 💡

- **Freigabestufe aus dem Sector** (`Safetycard.cs:24-34`):
  ```csharp
  releaseLevel = Sector switch
  {
      Sector.HR       => ReleaseLevel.red,
      Sector.logistic => ReleaseLevel.blue,
      ...
      _               => ReleaseLevel.none
  };
  ```
  Der `switch` schaut, welchen Wert `Sector` hat, und liefert die passende Stufe. `_` fängt alles andere ab (z.B. `Sector.none`). Wichtig ist der Aufruf **nach** `Sector = sector;` im Konstruktor – sonst wäre `Sector` noch `0` (= `HR`) und jede Karte `red`.

- **Karte vs. Mitarbeiter**: Die **Karte** sagt, *wohin* jemand darf (Stufe). Der **Mitarbeiter** sagt, *ob er gerade da sein kann* (Urlaub/Dienstreise). Zugang gibt es nur, wenn **beides** passt – deshalb braucht `CanAccess` beide Objekte.

- **Enum-Standardwert**: Ein Enum-Feld, das nie gesetzt wird, hat den Wert `0` – also den **ersten** Eintrag (`green` bzw. `HR`). Das gibt keinen Fehler, nur stilles Fehlverhalten.

- **`?.` und `??`** (in `GetCardReleaselevel`): `?.` ruft die Methode nur auf, wenn die Karte nicht `null` ist; `??` nimmt den rechten Wert, wenn links `null` herauskommt. Zusammen: „Stufe der Karte – oder `none`, falls es die Karte nicht gibt“.

- **Wer macht was (Single Responsibility)**:
  - `SafeteycardData` / `EmployeeData` → speichern und finden
  - `Terminal.CanAccess(karte, mitarbeiter)` → **nur prüfen**, gibt `true`/`false` zurück
  - `Terminal.Open()` / `Deny()` → Tür
  - `ControlSection.Challenge(kartenId)` → **steuert** den Ablauf und ruft alles der Reihe nach auf
  - `Program.Main` → legt Daten an und ruft für jede Karte `Challenge` auf

- **`static` vs. Instanz**: `SafeteycardData` ist `static` → genau eine, Aufruf über den Klassennamen. `EmployeeData` muss man mit `new EmployeeData()` erzeugen und an `ControlSection` weitergeben (z.B. über den Konstruktor).

## Empfohlene Reihenfolge

1. `Safetycard` um `GetOwnerId()` und `UpdateLastUsed()` ergänzen (Punkt 4).
2. `CanAccess(Safetycard, Employee)` mit Stufe **und** `IsAvailable()` bauen (Punkte 2, 5).
3. `ControlSection` bekommt `EmployeeData`, `Challenge(kartenId)` als kompletter Ablauf (Punkt 3).
4. `Main` mit Seed-Daten (10 Mitarbeiter, 4× `HR`) und Schleife über alle Karten füllen (Punkte 1, 7).
5. Testfälle aus Schritt 8 durchspielen und schriftlich festhalten.
6. Aufräumen (Punkte 8–14), optional Protokollsystem.
