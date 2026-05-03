# A1 Skigebiet-Simulator - Setup

## Was wird hier gemacht?

WPF-Programm, das mehrere Skifahrer (Threads) durch ein Skigebiet bewegt. Lifte sind durch Semaphoren auf eine bestimmte Anzahl gleichzeitiger Nutzer beschränkt. Wegfindung mit Dijkstra. Eigene UserControl `SkigebietAnzeige` mit DependencyProperties zur Visualisierung.

## Angabe-Dateien

| Datei | Was ist drin? | Wo hinkopieren? |
| :--- | :--- | :--- |
| `skigebiet.xml` | 7 Stationen, 6 Lifte, 8 Pisten | Projektroot, "In Ausgabeverzeichnis kopieren" auf "Bei neueren kopieren" |

## Format der XML-Datei

```xml
<Skigebiet>
  <Stationen>
    <Station Id="1" Name="Talstation" X="500" Y="600" />
    ...
  </Stationen>
  <Verbindungen>
    <Lift Von="1" Nach="2" Name="..." Kapazitaet="4" FahrzeitSek="60" />
    <Piste Von="3" Nach="2" Name="..." FahrzeitSek="40" />
    ...
  </Verbindungen>
</Skigebiet>
```

`X` und `Y` sind Pixelkoordinaten für die Anzeige (Canvas ca. 1000x800). `FahrzeitSek` ist die simulierte Fahrzeit in Sekunden. `Kapazitaet` gibt an wie viele Skifahrer einen Lift gleichzeitig nutzen können (für die Semaphore).

## Wichtige Klassen die du selbst schreibst

- `Knoten` (Datenklasse mit Id, Name, X, Y)
- `Kante` (Datenklasse mit VonId, NachId, Name, FahrzeitSek, IstLift, Kapazitaet, Semaphore)
- `Skifahrer` (Klasse mit aktueller Position, Ziel, Farbe, Thread, Statistik)
- `SkigebietAnzeige : UserControl` (deine Custom Control mit den DependencyProperties)
- `DijkstraHelper` (statische Klasse mit Methode `KuerzesteRoute(start, ziel, knoten, kanten)`)

## Tipps

- Die Semaphore pro Lift wird mit `new SemaphoreSlim(kapazitaet)` initialisiert
- Vor Lift-Benutzung: `await semaphore.WaitAsync()` (oder `semaphore.Wait()`), nach dem Lift `semaphore.Release()`
- Während des Wartens den Skifahrer in eine Warteschlangen-Liste eintragen, damit die Anzeige ihn dort zeigen kann
- Skifahrer-Bewegung im Thread per `Thread.Sleep(fahrzeit_in_ms)` simulieren
- UI-Updates aus dem Thread heraus: `Application.Current.Dispatcher.Invoke(() => ...)`
- Fahrzeit-Skalierung: 1 Sekunde im Skigebiet = z.B. 100 ms real (sonst dauert Tests zu lange)
- Threads sauber beenden: `CancellationToken` oder `volatile bool _running` checken in der Hauptschleife
- Dijkstra braucht eine `PriorityQueue<Knoten, double>` (.NET 6+) oder eine sortierte `List<>`

## DependencyProperty-Pattern (für SkigebietAnzeige)

```csharp
public static readonly DependencyProperty SkifahrerProperty =
    DependencyProperty.Register(
        nameof(Skifahrer),
        typeof(IEnumerable<Skifahrer>),
        typeof(SkigebietAnzeige),
        new PropertyMetadata(null, OnDataChanged));

public IEnumerable<Skifahrer> Skifahrer
{
    get => (IEnumerable<Skifahrer>)GetValue(SkifahrerProperty);
    set => SetValue(SkifahrerProperty, value);
}

private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
{
    ((SkigebietAnzeige)d).Neuzeichnen();
}
```

Bei jeder Bewegung ruft der Thread `Neuzeichnen()` (oder ähnliches) auf der UserControl auf, damit die Position aktualisiert wird. Alternativ: `INotifyPropertyChanged` auf den Skifahrer-Objekten und ein `DispatcherTimer` der alle 50 ms neu zeichnet.
