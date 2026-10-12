<a href="https://github.com/ciennce/Sicherheitsfreigabe/commit/0dbe541ec41f002a95f8ab7b0906adf44bb621ea">Claude usage:<a>

## [![Repography logo](https://images.repography.com/logo.svg)](https://repography.com) / Recent activity [![Time period](https://images.repography.com/159967951/ciennce/Sicherheitsfreigabe/recent-activity/TSNXUjia3BXS8LN7uyAiHNQySTopZ9CZiAcRgM9_FWA/yUM1eJXmkgO7vzdMwOdXWtgQkaeoJCh1u342uxA_JJ4_badge.svg)](https://repography.com)
[![Timeline graph](https://images.repography.com/159967951/ciennce/Sicherheitsfreigabe/recent-activity/TSNXUjia3BXS8LN7uyAiHNQySTopZ9CZiAcRgM9_FWA/yUM1eJXmkgO7vzdMwOdXWtgQkaeoJCh1u342uxA_JJ4_timeline.svg)](https://github.com/ciennce/Sicherheitsfreigabe/commits)
[![Top contributors](https://images.repography.com/159967951/ciennce/Sicherheitsfreigabe/recent-activity/TSNXUjia3BXS8LN7uyAiHNQySTopZ9CZiAcRgM9_FWA/yUM1eJXmkgO7vzdMwOdXWtgQkaeoJCh1u342uxA_JJ4_users.svg)](https://github.com/ciennce/Sicherheitsfreigabe/graphs/contributors)
# Fortschrittsbewertung – Sicherheitsfreigabe

Stand: 2026-10-12 (nach Commit `2e81891` „fixed more“). Abgleich von Code, Klassendiagramm (`Infos/Sicherheitsfreigabe-Klassendiagramm.drawio`) und Vorgehensweise.

## Kurzfazit

Die Prüflogik ist jetzt richtig: `Terminal.CanAccess(card, employee)` prüft **Kartenstufe und Verfügbarkeit** des Mitarbeiters – genau wie im Diagramm. Eine rote Karte im Urlaub oder auf Dienstreise wird damit abgelehnt. **Was noch fehlt, ist der Ablauf drumherum:** `Main` ruft die Steuereinheit nicht auf, das Menü in `Challange` stürzt bei jeder Eingabe ab (`ReadKey().ToString()`), und `ControlSection` erzeugt sich eine **eigene, leere** `EmployeeData` statt die aus `Main` zu benutzen.

## Abgleich mit der 10-Schritte-Vorgehensweise

| # | Schritt | Status |
|---|---------|--------|
| 1 | Modell entwerfen | ✅ Stufe wird aus `Sector` abgeleitet |
| 2 | Datenklassen (Mitarbeiter, Sicherheitskarte) | ⚠️ `Safetycard` fehlen `GetOwnerId()` / `UpdateLastUsed()` |
| 3 | Datenbankklassen | ✅ funktionsfähig |
| 4 | Seed-Daten | ⚠️ angefangen (1 Mitarbeiter, 1 Karte von mind. 10) |
| 5 | Terminal | ✅ `CanAccess(karte, mitarbeiter)` wie im Diagramm |
| 6 | Steuereinheit | ⚠️ eigene leere `EmployeeData`, Menü-Eingabe kaputt, Ablauf laut Diagramm fehlt |
| 7 | Main/Programmablauf | ⚠️ Daten werden angelegt, aber `ControlSection` nie benutzt |
| 8 | Tests | ⚠️ erste Testoptionen, noch nicht lauffähig |
| 9 | Protokollsystem (optional) | ❌ offen (ok, optional) |
| 10 | Abgabe | – |

## Was verbessert wurde ✅

Seit dem letzten Stand:

- **`CanAccess(Safetycard card, Employee employee)`** in `ITerminal` und `Terminal` – Signatur wie im Diagramm.
- **Echte Prüfung**: `card.GetReleaseLevel() == ReleaseLevel.red && employee.IsAvailable()` – Kartenstufe **und** Urlaub/Dienstreise werden geprüft. Damit sind die Testfälle „rote Karte, Urlaub“ und „rote Karte, Dienstreise“ logisch abgedeckt.
- **Terminal holt sich nichts mehr selbst aus der Datenbank** – es bekommt Karte und Mitarbeiter übergeben (Single Responsibility).
- **Zwischenvariable `result` entfernt**, `CanAccess` ist jetzt ein Einzeiler.
- **`ControlSection` hat ein Feld für `EmployeeData`** – erster Schritt, damit die Steuereinheit Mitarbeiter nachschlagen kann.

Davor schon erledigt:

- Erste Seed-Daten in `Main` (Mitarbeiterin „Anna“, `HR`-Karte 100).
- `SafeteycardData.Add` ohne unnötigen `ReleaseLevel`-Parameter; `Open()`/`Deny()` geben `string` zurück.
- Freigabestufe wird per `switch` aus dem `Sector` abgeleitet, im Konstruktor an der richtigen Stelle aufgerufen.
- `IsAvailable()` korrekt: `!IsOnVacation && !IsOnBusinessTrip`.
- `Sector` nur noch an der Karte; `Program.cs` mit `Main`; Syntaxfehler behoben; `ExpandoObject`/`dynamic` entfernt.
- `GetCardReleaselevel` gibt die echte Stufe zurück; `Employee` nicht mehr `abstract`, in `Sicherheitsfreigabe.Model`.
- `ReleaseLevel` / `EmployeeData` in PascalCase, `GetEmployee` gibt `Employee?` zurück.
- Redundante `releaseLevels`-Liste entfernt; Interface `ITerminal`; unidirektionale Beziehung Karte → Mitarbeiter.

## Noch zu fixen

### 🔴 Wichtig (Programm stürzt ab / tut nichts)

1. **`Console.ReadKey().ToString()` liefert nicht die Taste** (`ControlSection.cs:18`): `ReadKey()` gibt ein `ConsoleKeyInfo` zurück, dessen `ToString()` nur `"System.ConsoleKeyInfo"` ergibt. Keiner der Fälle `"1"`–`"4"` passt → **jede Eingabe wirft `Exception("Wrong input.")`**. Fix:
   ```csharp
   var input = Console.ReadKey().KeyChar.ToString();
   ```

2. **`ControlSection` benutzt eine eigene, leere Mitarbeiter-Datenbank** (`ControlSection.cs:13`): `employeeData = new();` erzeugt eine **neue** `EmployeeData`, in der niemand drin ist. Die Mitarbeiter aus `Main` landen in einer **anderen** Instanz. `GetEmployee` würde hier also immer `null` liefern. Lösung: die Datenbank aus `Main` übergeben:
   ```csharp
   public ControlSection(EmployeeData pEmployeeData)
   {
       terminal = new();
       employeeData = pEmployeeData;
   }
   ```
   ```csharp
   // in Main
   var control = new ControlSection(employees);
   ```

3. **`Main` benutzt die Steuereinheit nicht** (`Program.cs:7-17`): Es wird keine `ControlSection` erzeugt und nichts aufgerufen – das Programm endet sofort ohne Ausgabe.

4. **`Control()` gibt nichts aus** (`ControlSection.cs:32-42`): `terminal.Open()`/`Deny()` geben einen `string` zurück, der hier verworfen wird. Entweder `Console.WriteLine(terminal.Open());` oder `Control` gibt den Text zurück. Außerdem ist `employeeData.GetEmployee(employee.GetId());` in Zeile 37 wirkungslos – das Ergebnis wird nicht benutzt, und den Mitarbeiter hat man an der Stelle ja schon.

### 🟠 Aufbau / fehlende Teile

5. **`Challenge` soll nur die Karten-Id bekommen** (`ControlSection.cs:16`): Aktuell `Challange(int id, Safetycard card, Employee employee)` – der Aufrufer muss Karte und Mitarbeiter also schon selbst herausgesucht haben. Genau das ist aber die Aufgabe der Steuereinheit. Laut Diagramm `Challenge(kartenId)`:
   ```csharp
   public string Challenge(int cardId)
   {
       var card = SafeteycardData.GetCard(cardId);
       if (card == null) return terminal.Deny();               // unbekannte Karte

       var employee = employeeData.GetEmployee(card.GetOwnerId());
       if (employee == null) return terminal.Deny();           // Besitzer unbekannt

       card.UpdateLastUsed(DateTime.Now);
       return terminal.CanAccess(card, employee) ? terminal.Open() : terminal.Deny();
   }
   ```
   Dafür braucht `Safetycard` noch `GetOwnerId()` und `UpdateLastUsed()` (Punkt 7). Das **Menü** (welche Karte testen?) gehört dann nach `Main`. Im aktuellen Menü kann man mit „3“ die Tür öffnen, **ohne** dass irgendetwas geprüft wird.

6. **`throw` bei falscher Eingabe** (`ControlSection.cs:26`): Ein Tippfehler beendet das ganze Programm. Für ein Menü besser eine Meldung (`_ => "Ungültige Eingabe"`) und nochmal fragen.

7. **`Safetycard` fehlen Zugriffsmethoden**: Kein Getter für `ownerId` – deshalb kann die Steuereinheit den Besitzer nicht selbst finden. Außerdem keine Möglichkeit, `lastUsed` zu aktualisieren. Ergänzen:
   ```csharp
   public int GetOwnerId() => ownerId;
   public void UpdateLastUsed(DateTime pTime) => lastUsed = pTime;
   ```

8. **`GetCard(id)?.ToString()` zeigt nur den Klassennamen** (`ControlSection.cs:22`): Ohne eigene `ToString()` kommt `"Sicherheitsfreigabe.Safetycard"` heraus. In `Safetycard` ergänzen:
   ```csharp
   public override string ToString()
       => $"Karte {cardId}, Besitzer {ownerId}, Stufe {releaseLevel}, Bereich {Sector}";
   ```

9. **Seed-Daten unvollständig**: Mindestens 10 Mitarbeiter mit Karten, davon 4 mit `Sector.HR` (= rot): 1 Urlaub, 1 Dienstreise, 2 anwesend. Kleinigkeit: In `Main` gibt es schon `now`, trotzdem wird bei der Karte nochmal `DateTime.Now` verwendet.

### 🟡 Abweichungen vom Diagramm / Code-Qualität

10. **Enum-Werte** (`ReleaseLevel.cs`): Diagramm `Gruen, Gelb, Rot`, Code `green, blue, red, none` – „Gelb“ ist immer noch `blue`.
11. **Terminal fest auf `red`**: Für mehrere Türen könnte das Terminal eine Mindeststufe im Konstruktor bekommen. Optional – für die Aufgabe reicht ein rotes Terminal.
12. **`Employee.GetById` überflüssig**: Das Dictionary liefert schon den richtigen Mitarbeiter, `GetEmployee` kann direkt `employee` zurückgeben.
13. **Uneinheitlicher Lebenszyklus**: `SafeteycardData` ist `static`, `EmployeeData` eine normale Klasse. Einheitlich entscheiden.
14. **Benennung**: `SafeteycardData` (Tippfehler) in `Safecard.Data.cs`; `EmployeeData` in `Employee.data.cs`; `Challange` statt `Challenge`; `GetSafetycard()` liefert die Karten-ID → `GetCardId()`; Konstruktor-Parameter `sector` statt `pSector`. Innerhalb von `SafeteycardData` reicht `GetCard(Id)` statt `SafeteycardData.GetCard(Id)`.
15. **Diagramm aktualisieren**: `HiredDate`/`Birthday`, `Sector` (an der Karte), `ITerminal`, `none`, die Ableitung Sector → Freigabestufe und die `string`-Rückgabe von `Open()`/`Deny()` sind im Code, aber nicht im Klassendiagramm.

## Zum Verstehen 💡

- **`new` erzeugt immer ein neues, eigenes Objekt**: `new EmployeeData()` in `Main` und `new EmployeeData()` in `ControlSection` sind **zwei verschiedene** Datenbanken. Was man in die eine hineinlegt, ist in der anderen nicht drin. Wenn mehrere Klassen dieselben Daten sehen sollen, erzeugt man das Objekt **einmal** (in `Main`) und gibt es weiter, z.B. über den Konstruktor. Bei `SafeteycardData` fällt das Problem nicht auf, weil die Klasse `static` ist – davon gibt es automatisch nur eine.

- **`Console.ReadKey()` vs. `Console.ReadLine()`**:
  - `ReadKey()` wartet auf **eine** Taste und gibt ein `ConsoleKeyInfo` zurück. Das Zeichen steckt in `.KeyChar` (z.B. `'1'`).
  - `ReadLine()` wartet auf Enter und gibt den eingegebenen Text als `string` zurück – praktisch für Karten-Ids wie `100`.

- **`ToString()`**: Ohne eigenes `public override string ToString()` kommt nur der Klassenname heraus – deshalb `"System.ConsoleKeyInfo"` bzw. `"Sicherheitsfreigabe.Safetycard"`.

- **Rückgabewert vs. Ausgabe**: Eine Methode, die einen `string` zurückgibt, schreibt **nichts** auf die Konsole. Erst `Console.WriteLine(...)` zeigt ihn an. Wird der Rückgabewert nicht benutzt (`terminal.Open();`), passiert sichtbar nichts.

- **Karte vs. Mitarbeiter** (in `CanAccess`): Die **Karte** sagt, *wohin* jemand darf (Stufe). Der **Mitarbeiter** sagt, *ob er gerade da sein kann* (Urlaub/Dienstreise). `&&` heißt: Zugang nur, wenn **beides** stimmt.

- **Freigabestufe aus dem Sector**: Der `switch` in `AddReleaseLevel()` liefert zu jedem `Sector` die passende Stufe, `_` fängt den Rest ab. Der Aufruf muss **nach** `Sector = sector;` im Konstruktor stehen.

- **Wer macht was (Single Responsibility)**:
  - `SafeteycardData` / `EmployeeData` → speichern und finden
  - `Terminal.CanAccess(karte, mitarbeiter)` → **nur prüfen** ✅
  - `Terminal.Open()` / `Deny()` → Tür (Text)
  - `ControlSection.Challenge(kartenId)` → **sucht** Karte und Mitarbeiter, fragt das Terminal, aktualisiert `lastUsed`
  - `Program.Main` → legt Daten an, ggf. Menü, ruft für jede Karte `Challenge` auf und gibt das Ergebnis aus

## Empfohlene Reihenfolge

1. `ReadKey().KeyChar` fixen, damit das Menü nicht mehr abstürzt (Punkt 1).
2. `EmployeeData` aus `Main` per Konstruktor an `ControlSection` übergeben (Punkt 2).
3. `Safetycard` um `GetOwnerId()`, `UpdateLastUsed()` und `ToString()` ergänzen (Punkte 7, 8).
4. `Challenge(int cardId)` als kompletten Ablauf bauen, Menü nach `Main` (Punkte 4, 5, 6).
5. In `Main` `ControlSection` erzeugen, `Challenge` aufrufen und Ergebnis ausgeben (Punkt 3).
6. Seed-Daten auf 10 Mitarbeiter (4× `HR`) erweitern (Punkt 9).
7. Testfälle aus Schritt 8 durchspielen und schriftlich festhalten.
8. Aufräumen (Punkte 10–15), optional Protokollsystem.
