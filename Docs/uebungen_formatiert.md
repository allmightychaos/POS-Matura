# Multithreaded Counter

## Abschlussbedingungen

Erstellen Sie ein neues Windows-Konsolenprojekt.

### Aufgabe

Erstellen Sie eine Klasse, in der mehrere Threads (min. 3) gemeinsam einen Counter hochzählen. Jeder Thread soll eine aufsteigende ID haben (beginnend mit 2) und jedes mal bevor der Thread den Counter erhöht, soll folgender Code ausgeführt werden:

```csharp
if (counter % id == 0)
{
    Console.WriteLine("ID: {0,3} Counter: {1,8} Modulo: {2}", id, counter, counter % id);
}
```

Die Anzahl der Threads und wie oft jeder Thread den Counter erhöht, soll in den Befehlszeilenargumenten einstellbar sein. Starten Sie möglichst alle Threads gleichzeitig.

### Analyse

Starten Sie das Programm mehrmals und analysieren Sie die Ausgabe.

---

# Primzahlberechnung

## Abschlussbedingungen

Im folgenden Code werden Primzahlen berechnet. Ihre Aufgabe ist es, aus diesem Code eine Multithreaded Version zu entwickeln.

### Anforderungen

- Die Anzahl der Threads und der Höchstwert der Primzahlen soll über Befehlszeilenargumente einstellbar sein.
- Testen Sie das Programm mit verschiedenen Höchstwerten und Threadanzahlen. (Zumindest einmal mit einer Anzahl, die den Prozessoren des Systems entspricht)
- Erzeugen Sie mit den Ergebnissen eine Tabelle, die die Zeiten gegen die der Single-Thread-Version vergleicht (manuell z.B. in Excel oder auch im Programm).

### Single-Threaded Code-Vorlage

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Diagnostics;

namespace Primzahlen
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch watch = new Stopwatch();
            int maxPrim = 0;
            int number = 0;
            int tests = 0;
            watch.Start();
            Prim(1600000, out maxPrim, out number, out tests);
            watch.Stop();
            Console.WriteLine("Es wurden {0} Primzahlen gefunden", number);
            Console.WriteLine("Die höchste gefundene Primzahl ist {0}", maxPrim);
            Console.WriteLine("Die Laufzeit betrug {0:F0} Millisekungen", watch.ElapsedMilliseconds);
            Console.WriteLine("Es wurden {0} Vergleiche durchgeführt", tests);
        }

        private static void Prim(int max, out int maxPrim, out int number, out int tests)
        {
            List<int> prims = new List<int>();
            int i = 5;
            tests = 0;
            prims.Add(2);
            prims.Add(3);
            while (i < max)
            {
                int maxTeiler = (int)Math.Sqrt(i) + 1;
                int j = 0;
                while (true)
                {
                    int n = prims[j];
                    int rest = (i % n);
                    ++tests;
                    if (rest == 0)
                        break; //keine Primzahl
                    if(n >= maxTeiler)
                    {
                        prims.Add(i);
                        break;
                    }
                    ++j;
                }
                i += 2;
            }
            number = prims.Count;
            maxPrim = prims[number - 1];
        }
    }
}
```

### Beispieltabelle

| max Primzahl | nr. Primzahlen | ein Thread | zwei Threads | vier Threads |
| :----------- | :------------- | :--------- | :----------- | :----------- |
| 100000       | 9592           | 44ms       | 60ms         | 80ms         |
| 200000       | 17984          | 95ms       | 98ms         | 104ms        |
| 400000       | 33860          | 223ms      | 227ms        | 230ms        |
| ...          | ...            | ...        | ...          | ...          |
| 51200000     | 3068712        | 106000ms   | 61300ms      | 32940ms      |

---

# Die speisenden Philosophen

## Abschlussbedingungen

Es sitzen fünf Philosophen an einem runden Tisch, und jeder hat einen Teller mit Spaghetti vor sich. Zum Essen von Spaghetti benötigt jeder Philosoph zwei Gabeln. Allerdings waren im Haushalt nur fünf Gabeln vorhanden, die nun zwischen den Tellern liegen. Die Philosophen können also nicht gleichzeitig speisen.

Die Philosophen sitzen am Tisch und denken über philosophische Probleme nach. Wenn einer hungrig wird, greift er zuerst die Gabel links von seinem Teller, dann die auf der rechten Seite und beginnt zu essen. Wenn er satt ist, legt er die Gabeln wieder zurück und beginnt wieder zu denken. Sollte eine Gabel nicht an ihrem Platz liegen, wenn der Philosoph sie aufnehmen möchte, so wartet er, bis die Gabel wieder verfügbar ist.

### Aufgabe

Erstellen Sie eine neue **Windows-WPF-Anwendung**. Erstellen Sie Steuerelemente für folgende Punkte:

- Die Namen der 5 Philosophen (frei wählbar)
- Den Status jedes Philosophen (Label oder TextBox)
- Die Durchschnittszeit die alle Philosophen denken (default 1000)
- Die Varianz der Denkzeit (default 200)
- Die Durchschnittszeit die alle Philosophen essen (default 200)
- Die Varianz der Esszeit (default 40)
- Die Zeit die die Philosophen zum Aufnehmen der Gabel brauchen (default 40)
- Ein Label das angibt das alle Zeiten in ms sind
- Einen Start- und einen Stopp-Button

### Implementierungsdetails

- Die Zeit, die jeder Philosoph denkt/isst, ist wie folgt definiert: `Durchschnittszeit +/- Varianz`. Achten Sie darauf, dass die Zeit nicht negativ wird.
- Planen und erstellen Sie eine Klasse `Philosopher` und eine Klasse `Fork`.
- Stellen Sie sicher, dass keine zwei Philosophen die gleiche Gabel nehmen.
- Zur besseren Veranschaulichung soll die TextBox mit der Statusmeldung farbig untermalt werden:
    - **Weiß:** beim Denken
    - **Grün:** beim Essen
    - **Rot:** beim Warten
- Um die Zeiten abzuwarten, verwenden Sie die `Thread.Sleep(int ms)` Methode.
- **Stopp-Button:** Wenn der Stopp-Button gedrückt wird, sollen die Philosophen alle Aktionen abbrechen. Da `Thread.Abort()` in .NET Core nicht mehr unterstützt wird, verwenden Sie eine `bool` Eigenschaft in den Philosophen, um die Schleifen zu stoppen, und rufen Sie `Thread.Interrupt()` auf, um Blockierungen aufzuheben.

### Hinweise

**Namespaces:**

```csharp
using System.Windows.Controls;
using System.Windows.Media;
```

**Ändern der Steuerelemente aus einem anderen Thread:**
_Ausführlicher Code:_

```csharp
try
{
    textbox.Dispatcher.BeginInvoke(
        System.Windows.Threading.DispatcherPriority.Normal,
        new System.Windows.Threading.DispatcherOperationCallback(delegate
        {
            // eigentliche Änderungen
            textbox.Text = "denkt";
            textbox.Background = Brushes.White;
            textbox.UpdateLayout();
            return null;
        }), null);
}
catch (Exception ex)
{
    System.Diagnostics.Debug.WriteLine(ex.ToString());
}
```

_Kurzvariante mit Lambda:_

```csharp
textbox.Dispatcher.Invoke(new Action(() =>
{
    textbox.Text = "denkt";
    textbox.Background = Brushes.White;
}));
```

**Zusatzhinweise:**

- Verwendung von `Random` in C#.
- Bilder in WPF anzeigen.

---

# Floodfill

## Abschlussbedingungen

Flutfüllung bzw. **Floodfill** ist ein Begriff aus der Computergrafik. Es ist ein einfacher Algorithmus, um Flächen zusammenhängender Pixel einer Farbe in einem digitalen Bild zu erfassen und mit einer neuen Farbe zu füllen.

### Algorithmus

Ausgehend von einem Pixel innerhalb der Fläche werden jeweils dessen Nachbarpixel darauf getestet, ob diese Nachbarpixel auch die alte Farbe enthalten. Jedes gefundene Pixel mit der alten Farbe wird dabei sofort durch die neue Farbe ersetzt.

Zwei Varianten sind gängig:

1.  Untersuchung der 4 benachbarten Pixel (oben, unten, links, rechts).
2.  Untersuchung der 8 benachbarten Pixel (inklusive Diagonalen).

### Aufgabe

In der Angabe "Floodfill Projekt" ist bereits ein Projekt vorbereitet mit einem Konturenbild. In der `LeftMouseButtonDown`-Funktion soll nun ein neuer Thread gestartet werden, der die gewählte Fläche mit dem Floodfill-Algorithmus einfärbt.

---

# Drei Raucher

## Abschlussbedingungen

### Problem-Beschreibung

Ein Tabakwarenhändler und drei Raucher sitzen zusammen an einem Tisch. Zum Rauchen benötigen die Raucher folgende Dinge: Tabak, Zigarettenpapier und Streichhölzer.

- **Raucher A:** unendlich viel Tabak
- **Raucher B:** unendlich viel Zigarettenpapier
- **Raucher C:** unendlich viele Streichhölzer
- **Händler:** unendlich Vorrat an allen drei Zutaten

Ist der Tisch leer, wählt der Händler zwei der drei Zutaten zufällig aus und legt sie auf den Tisch. Der passende Raucher (der die dritte Zutat hat) kann nun eine Zigarette drehen und rauchen. Jedes Mal, wenn der Tisch leer ist, legt der Händler neues Material nach, andernfalls wartet er. Ein Raucher kann erst wieder tätig werden, wenn er fertig geraucht hat.

### Aufgabe

- Entwickeln Sie eine geeignete GUI. Die Zeit für das Rauchen soll einstellbar sein (ähnlich den Philosophen).
- Der Zustand der einzelnen Raucher muss immer dargestellt werden.
- Stellen Sie sicher, dass nie zwei Personen (Raucher oder Händler) gleichzeitig auf den Tisch zugreifen.
- Verwenden Sie `Wait / Pulse`, wenn eine Person nicht handeln kann.

### Hinweise

- Für die Ressourcen kann ein `Enum` verwendet werden.

### Erweiterung

Implementieren Sie eine Möglichkeit, den **Deadlock** in diesem Beispiel zu vermeiden, zu verhindern oder zu erkennen und zu beheben. Dokumentieren Sie Ihre Änderungen im Code. Die Effizienz (Zigaretten pro Zeit) soll dabei möglichst wenig beeinträchtigt werden.

---

# Achterbahn

## Abschlussbedingungen

Angenommen, es gibt `n` Passagiere und eine Achterbahn mit einem Wagen mit Platz für `x` Passagiere (`x < n`).

### Ablauf

Die Passagiere stellen sich immer wieder an. Der Wagen fährt nur los, wenn er voll besetzt ist. Nach einer Fahrt wandert jeder Passagier im Park umher, bevor er zurückkehrt. Zur Sicherheit fährt der Wagen nur `t` Fahrten und wird dann abgeschaltet.

### Bedingungen (Threads)

- Der Wagen fährt immer mit genau `x` Passagieren.
- Keine Passagiere springen ab, während der Wagen fährt.
- Keine Passagiere steigen zu, während der Wagen fährt.
- Keine Passagiere stellen sich erneut an, solange sie mitfahren.

### Aufgabe

- Erstellen Sie eine GUI zur Visualisierung.
- Verwenden Sie geeignete Thread-Techniken.
- Eine neue Zeile in einer `ListBox` hinzufügen:

```csharp
listBox1.Items.Add("Neuer Text");
listBox1.Items.MoveCurrentToLast();
listBox1.ScrollIntoView(listBox1.Items.CurrentItem);
```

---

# Kreuzung

## Abschlussbedingungen

### Aufgabe

Simulator einer Kreuzung mit Autos. WPF-Projekt mit Klassen `Car` und `Crossroad`.

- **Start-Button:** Erzeugt `x` neue Autos (ID fortlaufend, zufällige Richtung: Nord, Süd, Ost, West). Jedes Auto ist ein eigener Thread (`drive` Methode).
- **GUI:** ListBoxen für jede Himmelsrichtung und die Kreuzung selbst. Autos werden mit TextBoxen visualisiert (ID, Richtung, Status/Farbe).
- **Zeiten:** Autos erreichen die Kreuzung nach 1000–10000 ms. Die Überquerung (`cross` Funktion) dauert 1000 ms.
- **Auswahl:** Die Art der Kreuzung wird per ComboBox gewählt.

### Varianten

1.  **Kleine Kreuzung (Crossroad):** Nur Platz für ein Auto gleichzeitig. Keine Kollisionen.
2.  **Mehrere Spuren (LargeCrossroad):** Erbt von `Crossroad`. Überschreibt `cross` (`virtual`/`override`). Autos aus gegenüberliegenden Richtungen (N-S oder O-W) können gleichzeitig fahren, aber keine sich kreuzenden Richtungen.
3.  **Ampel (AmpelCrossroad):** Erbt von `Crossroad`. Ampel-Thread: 3s N-S grün, 2s Pause, 3s O-W grün, 2s Pause. Einsatz von Signalen.
4.  **Sensor-Ampel (SensorAmpelCrossroad):** Erbt von `Crossroad`. Standardmäßig N-S grün (min 3s). Wenn ein Auto aus O oder W kommt: N-S stoppt, 2s Pause, O-W 3s grün, 2s Pause, dann zurück zu N-S.

### Erweiterung

Passen Sie die Kreuzungen an (z.B. Vorrangregeln, Abbieger).

---

# Image Rotator

## Abschlussbedingungen

Bearbeiten aller Bilder in einem Ordner.

### Details

- Ordnerwahl: Erst fix, dann per `FolderBrowserDialog`.
- Aktivierung von Windows Forms für den Dialog:

```csharp
FolderBrowserDialog dialog = new FolderBrowserDialog();
dialog.ShowDialog();
string folder = dialog.SelectedPath;
```

- **Optionen:** Horizontal/Vertikal spiegeln, Rotation (90°, 180°, 270°), JPEG-Qualität (30–100).
- **Parallelisierung:** Verwenden Sie einen **Threadpool**.

```csharp
ThreadPool.QueueUserWorkItem(new WaitCallback(Worker), data);
// ...
private void Worker(object a) {
    Dataobjekt data = a as Dataobjekt;
    // ...
}
```

- **Visualisierung:** Fortschrittsbalken (`Maximum`, `Value`).
- **Speichern:** In einem neuen Ordner.

### Bildbearbeitung Code

```csharp
JpegBitmapEncoder encoder = new JpegBitmapEncoder();
encoder.FlipHorizontal = false;
encoder.FlipVertical = false;
encoder.QualityLevel = 30;
encoder.Rotation = Rotation.Rotate90;
encoder.Frames.Add(BitmapFrame.Create(new Uri(loadpath, UriKind.Relative)));
FileStream stream = new FileStream(savepath, FileMode.Create);
encoder.Save(stream);
stream.Close();
```

---

# Primzahlengenerator

## Abschlussbedingungen

Berechnung einer frei wählbaren n-ten Primzahl mit dem Threadpool.

### Aufgabe

- WPF-Oberfläche für Benutzereingabe (z.B. "Berechne die 465. Primzahl").
- Berechnung in einem Threadpool-Thread (Logik aus der Primzahlberechnungs-Übung).
- **Warte-Animation:** Während der Berechnung soll ein Bild rotieren (Storyboard).

### WPF Animation (XAML)

```xml
<Image Name="image1" Stretch="Fill" Width="124" Height="124" Source="loading_big.png">
    <Image.RenderTransform>
        <RotateTransform Angle="0" CenterX="62" CenterY="62" />
    </Image.RenderTransform>
</Image>

<Window.Resources>
    <Storyboard x:Key="loadingRotation">
        <DoubleAnimation Storyboard.TargetName="image1"
                         Storyboard.TargetProperty="(Image.RenderTransform).(RotateTransform.Angle)"
                         From="0" To="360" Duration="0:0:2.0" RepeatBehavior="Forever" />
    </Storyboard>
</Window.Resources>
```

### Storyboard Steuerung (C#)

```csharp
using System.Windows.Media.Animation;
// ...
Storyboard r = (Storyboard)FindResource("loadingRotation");
r.Begin(this, true); // Start
r.Stop(this);       // Stop
```

### Erweiterung

Verwendung einer `ListView` zur Anzeige der Ergebnisse in Tabellenform. Unterstützung für mehrfache gleichzeitige Berechnungen. Animation endet, wenn alle Berechnungen fertig sind.

---

# WPF Formen

## Abschlussbedingungen

### Aufgabe: Klassenhierarchie

Entwickeln Sie eine Hierarchie für geometrische Formen (Basisklasse als Wurzel). Verwenden Sie deutsche Namen für Klassen und Eigenschaften (z.B. `Rechteck`, `Breite`, `Hoehe`).

- Formen: Rechteck, Quadrat, Kreis, Trapez, Parallelogramm, Dreieck, Sechseck (regelmäßig), Raute, Polygon (regelmäßig), Stern (regelmäßig), Ellipse.
- Einfache Konstruktion (z.B. Raute über Diagonalen).
- Erstellen Sie ein **UML-Klassendiagramm**.
- Fügen Sie alle Formen mindestens einmal in ein `Canvas` ein.

### Hinweise

- Verwenden Sie `PathSegment` (z.B. `LineSegment`, `ArcSegment`).
- Regelmäßige Polygone/Sterne über Winkelfunktionen (`Math.Sin`/`Cos` in Radiant/2\*PI).

---

# WPF Wecker

## Abschlussbedingungen

Countdown-Wecker (Eieruhr) als Benutzerdefiniertes Steuerelement (Custom Control).

### Features

- Zeiteinstellung (Minuten/Sekunden).
- Buttons: Zeit setzen, Starten, Pausieren, Zurücksetzen.
- Alarmton bei Ablauf.

### Code-Beispiel (Auszug)

```csharp
public class AlarmClockControl : Control
{
    static AlarmClockControl() {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(AlarmClockControl), new FrameworkPropertyMetadata(typeof(AlarmClockControl)));
    }
    // DependencyProperties: AlarmTime, AlarmSet, CurrentTime
    // RoutedEvent: Alarm
    // Methods: RingAlarm (SoundPlayer), OnApplyTemplate (Event Binding)
}
```

### Windows Forms Integration

Fügen Sie `<UseWindowsForms>true</UseWindowsForms>` in die Projektdatei ein, um z.B. den `DateTimePicker` zu verwenden.

---

# WPF Indicator

## Abschlussbedingungen

Ein Indicator (Meter) als Custom Control zur Visualisierung von Füllständen.

### Anforderungen

- Bilder müssen als Ressourcen im Assembly eingebettet sein.
- **Eigenschaften:** `Minimum`, `Maximum`, `Value` (mit `PropertyChangedCallback`).
- Anzeige der Werte in `TextBlocks`.
- **Zeiger:** `RotateTransform` (Winkel 0° bis 287° basierend auf `Value`).

### Pack-URI / Bitmap Konvertierung

```csharp
// Pack-URI für Resource
BitmapFrame.Create(new Uri("pack://application:,,,/Projektname;component/resources/Bild.jpg", UriKind.RelativeOrAbsolute))

// Bitmap zu ImageSource
public static BitmapSource loadBitmap(System.Drawing.Bitmap source) {
    return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(source.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
}
```

### Erweiterung

Skalierbares Template.

---

# WPF System Monitor

## Abschlussbedingungen

Erstellung eines SystemMonitors (transparent, rahmenlos) zur Darstellung von Systemwerten.

### Features

- Fenster verschiebbar via `DragMove()` bei MouseDown.
- Hintergrundthread für Updates.
- **System-Tray:** Steuerung über `WPF NotifyIcon` (Sichtbarkeit, Schließen, Kontextmenü).
- **SystemData-Klasse:** Abfrage von CPU (+%), RAM (Physikalisch/Virtuell), Disk, Net mittels `PerformanceCounter` und `ManagementObjectSearcher` (System.Management).

### Erweiterung

Umschalten zwischen aktuellem Wert (Indicator) und Verlauf (Graph) via Kontextmenü.

---

# WPF Login / Registration

## Abschlussbedingungen

Zwei Custom Controls in einer Library: `Login` und `Registration`.

### Features

- **Login:** E-Mail/Benutzername, Passwort, Button für Wechsel zur Registrierung. Validierung (Regex/Leere Felder).
- **Registration:** Vorname, Nachname, E-Mail, Benutzername (optional), Passwort (doppelt), Adresse, Reset/Cancel-Buttons.
- **WPF-Test-App:** Schaltet Sichtbarkeit zwischen den Controls um und reagiert auf Events (Login/Register) mit einer MessageBox.

### UI-Tricks

- Button als Hyperlink via `ControlTemplate` und `Style.Triggers`.
- Validierung mit `Regex`.
- Passwort-Eingabe via `PasswordBox`.

---

# WPF WordPad

## Abschlussbedingungen

Textbearbeitungsprogramm mit `RichTextBox` und `Ribbon-Control`.

### Anforderungen

- Grundfunktionen: Copy, Paste, Cut, Bold, Italic,...
- Styles und Trigger für einheitliches Design.
- Verwendung von `Fluent.Ribbon` (NuGet) oder integrierten Ribbons (.NET Framework).
- **Commands:** Nutzung von `EditingCommands.ToggleBold` etc.
- Laden/Speichern als XAML.
- **Extended WPF Toolkit:** Für zusätzliche Steuerelemente.

---

# WPF Video-Player

## Abschlussbedingungen

Skalierbarer Videoplayer mit Playlisten und Verlauf.

### Features

- **Komponenten:** DockPanel, MediaElement, Steuerelemente (Play, Pause, Volume, Fortschritt).
- **Listen:** Playlist und History mit `DataTemplates`. Doppelklick startet Video.
- **Logik:** Nächstes Video startet automatisch.
- `LoadedBehavior="Manual"` für manuelle Steuerung.

---

# WPF Einkaufslistengenerator

## Abschlussbedingungen

Einkaufsliste mit starkem Fokus auf **LINQ**.

### Anforderungen

- Einlesen einer CSV (z.B. `LINQtoCSVCore`).
- Cascading ComboBoxes (Produktgruppe -> Produkte).
- Hinzufügen/Erhöhen der Anzahl (ObservableCollection).
- Menü: Löschen, Neu, Drucken, Speichern/Laden (XML).
- **Suchfunktion:** Filterung der Produkte per LINQ.
- **TreeView:** Gruppierte Darstellung der Liste.

---

# WPF Solitär

## Abschlussbedingungen

Das Brettspiel Solitär mit **Drag & Drop**.

### Anforderungen

- Modell im Hintergrund.
- Erkennung von Sieg (1 Stein) und Niederlage (keine Züge möglich).
- Wählbare Spielfeldvarianten.

### Erweiterung

Bilder statt geometrischer Formen für Steine und Felder.

---

# Waldwunder Verwaltung

## Abschlussbedingungen

Informationssystem für die Bundesforste.

### Features

- **GUI:** Skalierbar (responsive).
- **Daten:** SQLite mit ORM und LINQ.
- **Bilder:** Speicherung in Ordner `Images` (bei Duplikaten Umbenennung mit Index).
- **Funktionen:** Anlegen neuer Waldwunder (mit Dialog), Suchen nach Stichwort, Art oder Ort (Koordinaten-Radius ± 0,5).
- **Visualisierung:** ListBox + Karte mit Markierungen (Synchronisierung).

---

# WPF-Chat

## Abschlussbedingungen

Client-Server Anwendung für Chats.

### Features

- **Server:** Protokollierung (Aktivitäts-Log), SQLite-Datenbank.
- **Client:** Chat-Räume in Tabs, User-Login, Farbwahl, Profilbilder.
- **Bilder:** Übertragung via Base64-Strings in XML/DB.
- **Konfiguration:** Speichern der letzten Server-IP in XML.

---

# Robotersteuerung

## Abschlussbedingungen

Geometrisches Feld mit Roboter (Custom Control). Textbasierte Steuerung für Kinder.

### Anforderungen

- **Grammatik:** Erstellung einer ABNF-Grammatik (REPEAT, UNTIL, IF, MOVE, COLLECT).
- **Entwurf:** UML-Diagramm basierend auf dem **Interpreter-Pattern**.
- **Ausführung:** Parsen von Textdateien, visualisierte schrittweise Ausführung (1s Pause).
- Fehlermeldungen bei Syntaxfehlern.

---

# WPF Rubiks Cube

## Abschlussbedingungen

Interaktiver 3D-Würfel.

### Anforderungen

- Verstellen per Zufall oder per Buttons (Ebenen rotation).
- Blockierung von Eingaben während einer Animation.
- **Erweiterung:** Automatische Lösung im separaten Thread (Animationen schrittweise).

---

# Aussagenlogik-Parser

## Abschlussbedingungen

Parsen und Auswerten boolescher Formeln (∧, ⋁, ¬, →, ↔).

### Anforderungen

- ABNF Grammatik & Interpreter-Pattern Design.
- Generierung einer **Wahrheitstabelle** aus dem Syntaxbaum.
- Visualisierung mit einem **KV-Diagramm** (aus DLL).
- Formeloptimierung anhand des Diagramms.

---

# Kakuro

## Abschlussbedingungen

Logikrätsel (Summen-Sudoku) aus XML laden.

### Anforderungen

- Dynamisches `UniformGrid` (Inaktive Felder, Summenfelder, Eingabefelder).
- Validierung: Zahlen 1–9, keine Doppelten pro Summe.
- Fehler-Hervorhebung: Hintergrundfarbe "Salmon" bei Regelverstoß.

---

# Routenplanung (ISP)

## Abschlussbedingungen

Optimierung von Infrastruktur-Routen.

### Anforderungen

- SQLite-Datenbank mit ORM.
- Anzeige der Router in einem Graph-Control (DLL).
- Implementierung der Suche nach dem **kürzesten Weg** (z.B. Dijkstra).
- Visualisierung der Route im Graphen.

---

# Gomoku (Fünf-Gewinnt)

## Abschlussbedingungen

WPF-Umsetzung des Spiels.

### Anforderungen

- Modi: Mensch vs. Mensch, Mensch vs. Computer, Netzwerk (Server/Client).
- Architektur: **MVC-Pattern** mit austauschbaren Controllern.
- Dynamisches Spielfeld (`UniformGrid`).

---

# Ultimatives Osterhasen Hilfs-Programm

## Abschlussbedingungen

Geographische Auslieferungsplanung in Wiener Neustadt.

### Features

- **Aufgabe 1:** Registrierung (SQLite, ORM, LINQ). Koordinaten speichern.
- **Aufgabe 2:** Visualisierung der Wünsche auf einer Karte (statische Grenzen angegeben).
- **Aufgabe 3:** Clustering der Ostereier auf Helfer (Algorithmus zur Gruppierung nach Nähe).
- **Aufgabe 4:** Routenplanung (TSP - Traveling Salesman Problem) für jeden Helfer ab einem Startpunkt.

---

# Bilderverwaltungsprogramm

## Abschlussbedingungen

Albenbasierte Fotoverwaltung.

### Features

- **Datei:** Neues Album (Ordner), Bilder hinzufügen (aus ZIP entpacken), Verschieben, Löschen.
- **Bearbeiten:** Rotation (90°, 180°). Überschreiben der Dateien.
- **GUI:** Albenwahl per ComboBox, Gallerie-View mit `DataTemplate` (ListBox).
- Persistence: XML-Speicherung der Metadaten.

---

# Schiffe versenken

## Abschlussbedingungen

Netzwerkbasiertes Spiel nach **MVVM/MVC**.

### Anforderungen

- Zwei Views: Eigene Schiffe vs. Gegnerische Schiffe.
- Validierung beim Schiffe setzen.
- Netzwerk-Code zum Datenaustausch zwischen Clients.

---

# Taschenrechner

## Abschlussbedingungen

Wissenschaftlicher Rechner mit Formelauswertung.

### Anforderungen

- **Support:** Operatoren (+, -, \*, /, ^), Klammern, Variablen (x, y, z).
- **Ablauf:** Regex für Tokenizing -> Parser -> Interpreter-Pattern (Syntaxbaum).
- **Abfrage:** Falls Variablen verwendet werden, Werte vorher abfragen.
- Fehler-Highlighting bei falschen Formeln.
