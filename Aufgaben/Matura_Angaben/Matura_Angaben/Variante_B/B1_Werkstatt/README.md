# B1 Werkstatt-Simulator - Setup

## Was wird hier gemacht?

WPF-Programm mit mehreren Mechaniker-Threads, die Aufträge aus einer prioritisierten Warteschlange abarbeiten. Producer-Consumer-Pattern mit `Monitor.Wait` und `Monitor.PulseAll`. Anzahl der Mechaniker dynamisch über Slider einstellbar. Ein Auto-Optimizer fügt Mechaniker dynamisch hinzu/entfernt sie.

## Angabe-Dateien

**Keine.** Alle Daten werden zur Laufzeit generiert.

## Aufbau (Vorschlag)

### Auftrag.cs (Datenklasse)
```csharp
public class Auftrag
{
    public int Id { get; set; }
    public int Prioritaet { get; set; }       // 1-3, hoeher = wichtiger
    public int BearbeitungsdauerMs { get; set; }
    public DateTime Eingang { get; set; }
    public DateTime? Erledigt { get; set; }
    public TimeSpan? Wartezeit => Erledigt - Eingang;
}
```

### AuftragsZentrale.cs (Producer-Consumer)
```csharp
public class AuftragsZentrale
{
    private readonly List<Auftrag> _queue = new();
    private readonly object _lock = new();

    public void HinzufuegenAuftrag(Auftrag a)
    {
        lock (_lock)
        {
            _queue.Add(a);
            Monitor.PulseAll(_lock);
        }
    }

    public Auftrag NaechsterAuftrag(CancellationToken ct)
    {
        lock (_lock)
        {
            while (_queue.Count == 0)
            {
                if (ct.IsCancellationRequested) return null;
                Monitor.Wait(_lock, 100); // Timeout damit Cancellation greift
            }

            // Hoechste Prioritaet zuerst (bei gleicher: aelteste zuerst)
            var beste = _queue.OrderByDescending(x => x.Prioritaet)
                              .ThenBy(x => x.Eingang)
                              .First();
            _queue.Remove(beste);
            return beste;
        }
    }
}
```

### Mechaniker.cs (Worker-Thread)
```csharp
public class Mechaniker
{
    public int Id { get; }
    public string Status { get; private set; } = "idle";
    private readonly Thread _thread;
    private readonly CancellationTokenSource _cts = new();
    private readonly AuftragsZentrale _zentrale;

    public Mechaniker(int id, AuftragsZentrale zentrale)
    {
        Id = id;
        _zentrale = zentrale;
        _thread = new Thread(Arbeiten) { IsBackground = true };
        _thread.Start();
    }

    private void Arbeiten()
    {
        while (!_cts.IsCancellationRequested)
        {
            var auftrag = _zentrale.NaechsterAuftrag(_cts.Token);
            if (auftrag == null) break;

            Status = "arbeitend";
            // UI-Update via Dispatcher
            Thread.Sleep(auftrag.BearbeitungsdauerMs);
            auftrag.Erledigt = DateTime.Now;
            Status = "fertig";
        }
    }

    public void Beenden() => _cts.Cancel();
}
```

## Auto-Optimizer

Periodisch (z.B. alle 2 Sek per `DispatcherTimer`):

```csharp
var avgWartezeit = ErledigteAuftraege.Average(a => a.Wartezeit.Value.TotalSeconds);

if (avgWartezeit > 2.0 && Mechaniker.Count < 8)
    MechanikerHinzufuegen();
else if (avgWartezeit < 0.5 && Mechaniker.Count > 1)
    MechanikerEntfernen();
```

## Visualisierung-Tipp

Mechaniker als `Rectangle` in einem `WrapPanel` oder `StackPanel`. Farbe per Binding an `Status` (Converter: idle=grau, arbeitend=gelb, fertig=grün-für-eine-Sekunde-dann-grau).

Animation der Statusänderung mit `ColorAnimation` in einem `Storyboard`:

```xml
<Rectangle Width="60" Height="80" Fill="{Binding StatusFarbe}">
    <Rectangle.Resources>
        <Storyboard x:Key="GruenBlink">
            <ColorAnimation Storyboard.TargetProperty="(Fill).(SolidColorBrush.Color)"
                            From="LimeGreen" To="Gray" Duration="0:0:1"/>
        </Storyboard>
    </Rectangle.Resources>
</Rectangle>
```

## Saubere Beendigung

`MainWindow.Closing` Event: Auf alle Mechaniker `Beenden()` aufrufen, dann auf alle Threads warten (`Join()`). Wichtig: `Monitor.Wait` mit Timeout (siehe oben), sonst hängen die Threads ewig im Wait.
