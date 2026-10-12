<a href="https://github.com/ciennce/Sicherheitsfreigabe/commit/0dbe541ec41f002a95f8ab7b0906adf44bb621ea">Claude usage:<a>

## [![Repography logo](https://images.repography.com/logo.svg)](https://repography.com) / Recent activity [![Time period](https://images.repography.com/159967951/ciennce/Sicherheitsfreigabe/recent-activity/TSNXUjia3BXS8LN7uyAiHNQySTopZ9CZiAcRgM9_FWA/yUM1eJXmkgO7vzdMwOdXWtgQkaeoJCh1u342uxA_JJ4_badge.svg)](https://repography.com)
[![Timeline graph](https://images.repography.com/159967951/ciennce/Sicherheitsfreigabe/recent-activity/TSNXUjia3BXS8LN7uyAiHNQySTopZ9CZiAcRgM9_FWA/yUM1eJXmkgO7vzdMwOdXWtgQkaeoJCh1u342uxA_JJ4_timeline.svg)](https://github.com/ciennce/Sicherheitsfreigabe/commits)
[![Top contributors](https://images.repography.com/159967951/ciennce/Sicherheitsfreigabe/recent-activity/TSNXUjia3BXS8LN7uyAiHNQySTopZ9CZiAcRgM9_FWA/yUM1eJXmkgO7vzdMwOdXWtgQkaeoJCh1u342uxA_JJ4_users.svg)](https://github.com/ciennce/Sicherheitsfreigabe/graphs/contributors)
# Fortschrittsbewertung – Sicherheitsfreigabe

Stand: 2026-10-12 (nach Commit `3e850cc` „added a bit to program.cs and a few test options.“). Abgleich von Code, Klassendiagramm (`Infos/Sicherheitsfreigabe-Klassendiagramm.drawio`) und Vorgehensweise.

## Kurzfazit

`Main` legt jetzt den ersten Mitarbeiter und die erste Karte an, und `Challange` hat ein kleines Test-Menü bekommen. **Aber:** Das Menü kann nie funktionieren – `Console.ReadKey().ToString()` liefert nicht die gedrückte Taste, sondern den Text `"System.ConsoleKeyInfo"`, deshalb landet **jede** Eingabe im `throw` und das Programm stürzt ab. Außerdem wird `Challange` in `Main` noch gar nicht aufgerufen, und seit `Open()`/`Deny()` Strings zurückgeben, gibt `Control()` nichts mehr aus. Der Mitarbeiter (Urlaub/Dienstreise) wird weiterhin nicht geprüft.

## Abgleich mit der 10-Schritte-Vorgehensweise

| # | Schritt | Status |
|---|---------|--------|
| 1 | Modell entwerfen | ✅ Stufe wird aus `Sector` abgeleitet |
| 2 | Datenklassen (Mitarbeiter, Sicherheitskarte) | ⚠️ `Safetycard` fehlen `GetOwnerId()` / `UpdateLastUsed()` |
| 3 | Datenbankklassen | ✅ funktionsfähig |
| 4 | Seed-Daten | ⚠️ angefangen (1 Mitarbeiter, 1 Karte von mind. 10) |
| 5 | Terminal | ⚠️ ohne Mitarbeiter-Prüfung, Signatur weicht vom Diagramm ab |
| 6 | Steuereinheit | ⚠️ Test-Menü in `Challange`, Eingabe kaputt, Ablauf laut Diagramm fehlt |
| 7 | Main/Programmablauf | ⚠️ Daten werden angelegt, aber `ControlSection` nie benutzt |
| 8 | Tests | ⚠️ erste Testoptionen, noch nicht lauffähig |
| 9 | Protokollsystem (optional) | ❌ offen (ok, optional) |
| 10 | Abgabe | – |

## Was verbessert wurde ✅

Seit dem letzten Stand:

- **Erste Seed-Daten in `Main`**: `EmployeeData` wird erzeugt, Mitarbeiterin „Anna“ (Id 1) und eine `HR`-Karte (Id 100, Besitzer 1) werden angelegt.
- **Unbenutzter Parameter entfernt**: `SafeteycardData.Add(Safetycard card)` braucht kein `ReleaseLevel` mehr – die Stufe kommt aus der Karte selbst.
- **`Open()`/`Deny()` geben einen `string` zurück** statt selbst zu schreiben – damit kann der Aufrufer entscheiden, was mit dem Text passiert (gut für Tests).
- **Test-Menü in `Challange`** als erster Ansatz, die einzelnen Schritte auszuprobieren.

Davor schon erledigt:

- Freigabestufe wird per `switch` aus dem `Sector` abgeleitet (`Safetycard.AddReleaseLevel()`), im Konstruktor an der richtigen Stelle aufgerufen.
- `IsAvailable()` korrekt: `!IsOnVacation && !IsOnBusinessTrip`.
- `Sector` nur noch an der Karte, nicht mehr doppelt im `Employee`.
- `Program.cs` mit `Main`, Syntaxfehler in `Employee.cs` behoben, `ExpandoObject`/`dynamic` entfernt.
- `GetCardReleaselevel` gibt die echte Stufe zurück; `Employee` nicht mehr `abstract`, in `Sicherheitsfreigabe.Model`.
- `ReleaseLevel` / `EmployeeData` in PascalCase, `GetEmployee` gibt `Employee?` zurück.
- Redundante `releaseLevels`-Liste entfernt, `CanAccess` ruft `Open()`/`Deny()` nicht mehr selbst auf.
- Interface `ITerminal`, `ControlSection` angelegt, unidirektionale Beziehung Karte → Mitarbeiter.

## Noch zu fixen

### 🔴 Wichtig (Programm stürzt ab / tut nichts)

1. **`Console.ReadKey().ToString()` liefert nicht die Taste** (`ControlSection.cs:13`): `ReadKey()` gibt ein `ConsoleKeyInfo` zurück, und dessen `ToString()` ergibt nur den Typnamen `"System.ConsoleKeyInfo"`. Keiner der Fälle `"1"`–`"4"` passt jemals → **jede Eingabe wirft `Exception("Wrong input.")`** und das Programm stürzt ab. Fix:
   ```csharp
   var input = Console.ReadKey().KeyChar.ToString();
   ```

2. **`Main` benutzt die Steuereinheit nicht** (`Program.cs:7-17`): Daten werden angelegt, aber es wird keine `ControlSection` erzeugt und nichts aufgerufen – das Programm endet sofort ohne Ausgabe. Außerdem wird `employees` nirgends weitergegeben, `ControlSection` kann den Mitarbeiter also gar nicht finden. Ergänzen z.B.:
   ```csharp
   var control = new ControlSection(employees);
   Console.WriteLine(control.Challenge(100));
   ```

3. **`Control()` gibt nichts mehr aus** (`ControlSection.cs:27-36`): Seit `Open()`/`Deny()` einen `string` zurückgeben, wird der Text in `Control` einfach verworfen. Entweder `Console.WriteLine(terminal.Open());` oder `Control` gibt den Text als `string` zurück.

4. **Mitarbeiter wird nicht geprüft**: `IsAvailable()` ist fertig, wird aber nirgends aufgerufen. `CanAccess(id)` schaut nur auf die Kartenstufe → **Rote Karte + Urlaub/Dienstreise → Zugang gewährt**. Genau das sind die Testfälle aus Schritt 8.

### 🟠 Aufbau / fehlende Teile

5. **Menü gehört nicht in `Challenge`** (`ControlSection.cs:11-25`): Laut Diagramm ist `Challenge(kartenId)` der **automatische** Ablauf, wenn eine Karte ans Terminal gehalten wird. Im aktuellen Menü kann man mit „3“ die Tür öffnen, **ohne** dass irgendetwas geprüft wird – das wäre bei einer echten Tür eine Sicherheitslücke. Besser:
   - Das **Menü** (welche Karte soll getestet werden?) kommt nach `Main`.
   - `Challenge(kartenId)` macht dann immer den kompletten Ablauf:
     1. Karte nachschlagen (`GetCard`) – unbekannte Karte → `Deny()`
     2. Besitzer über `ownerId` in `EmployeeData` holen
     3. `terminal.CanAccess(karte, mitarbeiter)` fragen
     4. `Open()` bzw. `Deny()` zurückgeben/ausgeben
     5. `lastUsed` der Karte aktualisieren

6. **`throw` bei falscher Eingabe** (`ControlSection.cs:21`): Ein Tippfehler beendet das ganze Programm. Für ein Menü besser eine Meldung ausgeben (`_ => "Ungültige Eingabe"`) und nochmal fragen.

7. **`GetCard(id)?.ToString()` zeigt nur den Klassennamen** (`ControlSection.cs:17`): Ohne eigene `ToString()`-Methode kommt `"Sicherheitsfreigabe.Safetycard"` heraus. Wenn man die Karte anzeigen will, in `Safetycard` ergänzen:
   ```csharp
   public override string ToString()
       => $"Karte {cardId}, Besitzer {ownerId}, Stufe {releaseLevel}, Bereich {Sector}";
   ```

8. **`Safetycard` fehlen Zugriffsmethoden**: Kein Getter für `ownerId` (Steuereinheit kann den Besitzer nicht ermitteln) und keine Möglichkeit, `lastUsed` zu aktualisieren. Ergänzen z.B. `GetOwnerId()` und `UpdateLastUsed(DateTime)`.

9. **`Terminal.CanAccess` bekommt keinen Mitarbeiter** (`ITerminal.cs:5`, `Terminal.cs:5`): Signatur ist `CanAccess(int id)`, das Diagramm verlangt `CanAccess(karte, mitarbeiter)`. Vorschlag:
   ```csharp
   public bool CanAccess(Safetycard card, Employee employee)
       => card.GetReleaseLevel() == ReleaseLevel.red && employee.IsAvailable();
   ```

10. **Seed-Daten unvollständig**: Mindestens 10 Mitarbeiter mit Karten, davon 4 mit Stufe „rot“, also `Sector.HR` (1 Urlaub, 1 Dienstreise, 2 anwesend). Kleinigkeit: In `Main` gibt es schon `now`, trotzdem wird bei der Karte nochmal `DateTime.Now` verwendet.

### 🟡 Abweichungen vom Diagramm / Code-Qualität

11. **Enum-Werte** (`ReleaseLevel.cs`): Diagramm `Gruen, Gelb, Rot`, Code `green, blue, red, none` – „Gelb“ ist immer noch `blue`.
12. **`Employee.GetById` überflüssig**: Das Dictionary liefert schon den richtigen Mitarbeiter, `GetEmployee` kann direkt `employee` zurückgeben.
13. **`Terminal.CanAccess`**: Die Zwischenvariable `result` ist unnötig, den `switch` direkt zurückgeben.
14. **Uneinheitlicher Lebenszyklus**: `SafeteycardData` ist `static`, `EmployeeData` eine normale Klasse. Einheitlich entscheiden.
15. **Benennung**: `SafeteycardData` (Tippfehler) in `Safecard.Data.cs`; `EmployeeData` in `Employee.data.cs`; `Challange` statt `Challenge`; `GetSafetycard()` liefert die Karten-ID → `GetCardId()`; Konstruktor-Parameter `sector` statt `pSector`. Innerhalb von `SafeteycardData` reicht `GetCard(Id)` statt `SafeteycardData.GetCard(Id)`.
16. **Diagramm aktualisieren**: `HiredDate`/`Birthday`, `Sector` (an der Karte), `ITerminal`, `none`, die Ableitung Sector → Freigabestufe und die `string`-Rückgabe von `Open()`/`Deny()` sind im Code, aber nicht im Klassendiagramm.

## Zum Verstehen 💡

- **`Console.ReadKey()` vs. `Console.ReadLine()`**:
  - `ReadKey()` wartet auf **eine** Taste und gibt ein `ConsoleKeyInfo` zurück. Das Zeichen steckt in `.KeyChar` (z.B. `'1'`), die Taste in `.Key` (z.B. `ConsoleKey.D1`).
  - `ReadLine()` wartet auf Enter und gibt den ganzen eingegebenen Text als `string` zurück – praktisch, wenn man z.B. eine Karten-Id wie `100` eintippen will.

- **`ToString()`**: Jede Klasse hat `ToString()`. Wenn man sie nicht selbst überschreibt (`public override string ToString()`), kommt nur der Klassenname heraus. Deshalb liefert `Console.ReadKey().ToString()` `"System.ConsoleKeyInfo"` und `card.ToString()` `"Sicherheitsfreigabe.Safetycard"`.

- **Freigabestufe aus dem Sector** (`Safetycard.cs:24-34`): Der `switch` schaut, welchen Wert `Sector` hat, und liefert die passende Stufe; `_` fängt alles andere ab. Wichtig ist der Aufruf **nach** `Sector = sector;` im Konstruktor – sonst wäre `Sector` noch `0` (= `HR`) und jede Karte `red`.

- **Karte vs. Mitarbeiter**: Die **Karte** sagt, *wohin* jemand darf (Stufe). Der **Mitarbeiter** sagt, *ob er gerade da sein kann* (Urlaub/Dienstreise). Zugang gibt es nur, wenn **beides** passt – deshalb braucht `CanAccess` beide Objekte.

- **Rückgabewert vs. Ausgabe**: Eine Methode, die einen `string` zurückgibt, schreibt **nichts** auf die Konsole. Erst `Console.WriteLine(...)` zeigt den Text an. Wird der Rückgabewert nicht benutzt (`terminal.Open();`), passiert sichtbar gar nichts.

- **`?.` und `??`**: `?.` ruft die Methode nur auf, wenn das Objekt nicht `null` ist; `??` nimmt den rechten Wert, wenn links `null` herauskommt.

- **Wer macht was (Single Responsibility)**:
  - `SafeteycardData` / `EmployeeData` → speichern und finden
  - `Terminal.CanAccess(karte, mitarbeiter)` → **nur prüfen**, gibt `true`/`false` zurück
  - `Terminal.Open()` / `Deny()` → Tür (Text)
  - `ControlSection.Challenge(kartenId)` → **steuert** den Ablauf und ruft alles der Reihe nach auf
  - `Program.Main` → legt Daten an, ggf. Menü, ruft für jede Karte `Challenge` auf

## Empfohlene Reihenfolge

1. `ReadKey().KeyChar` fixen, damit das Programm nicht mehr abstürzt (Punkt 1).
2. In `Main` eine `ControlSection` erzeugen und `Challenge` aufrufen, Ergebnis mit `Console.WriteLine` ausgeben (Punkte 2, 3).
3. `Safetycard` um `GetOwnerId()`, `UpdateLastUsed()` und `ToString()` ergänzen (Punkte 7, 8).
4. `CanAccess(Safetycard, Employee)` mit Stufe **und** `IsAvailable()` bauen (Punkte 4, 9).
5. `ControlSection` bekommt `EmployeeData`, `Challenge(kartenId)` als kompletter Ablauf, Menü nach `Main` (Punkte 5, 6).
6. Seed-Daten auf 10 Mitarbeiter (4× `HR`) erweitern (Punkt 10).
7. Testfälle aus Schritt 8 durchspielen und schriftlich festhalten.
8. Aufräumen (Punkte 11–16), optional Protokollsystem.
