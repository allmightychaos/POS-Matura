# B3 Smart-Home-Skript - Setup

## Was wird hier gemacht?

WPF-Programm mit Server- und Client-Modus. Der Client schreibt Skripte in einer einfachen Smart-Home-Sprache, sendet sie über `Transfer<T>` an den Server. Der Server parst und führt sie aus, schickt Statusupdates an alle Clients zurück. Sensoren werden vom Server zufällig variiert.

## Angabe-Dateien

| Datei | Was ist drin? | Wo hinkopieren? |
| :--- | :--- | :--- |
| `Anweisung.cs` | Abstrakte Basisklasse für alle Anweisungen | Server- UND Client-Projektroot, "Hinzufügen > Vorhandenes Element" |
| `SmartHomeContext.cs` | State-Holder + Raum/Lampe/Sensor-Klassen | Server- UND Client-Projektroot, "Hinzufügen > Vorhandenes Element" |
| `raeume.xml` | 4 Räume mit Lampen und Sensoren | Nur Server-Projektroot, "In Ausgabeverzeichnis kopieren" |
| `beispiel_skript.txt` | Beispiel-Skript mit allen Konstrukten | Optional, beim Client zum Kopieren in TextBox |

Die Klassen `Raum`, `Lampe`, `Sensor` werden auch zum Versenden über das Netzwerk gebraucht (in `SmartHomeMessage`), deshalb gehört `SmartHomeContext.cs` in beide Projekte.

## Namespace

Beide C#-Dateien benutzen `SmartHome`. Wenn dein Projekt einen anderen Namespace hat, entweder anpassen oder `using SmartHome;` schreiben.

## Transfer_2026.cs

Wie bei A2: in beide Projekte kopieren. Namespace `Network`.

## SmartHomeMessage

```csharp
public class SmartHomeMessage
{
    public enum MsgType { SkriptSenden, LampenStatus, SensorWert, Fehler }

    public MsgType Typ { get; set; }
    public string Skript { get; set; }
    public List<Raum> RaeumeStatus { get; set; }
    public string Raum { get; set; }
    public string Element { get; set; }
    public double? Wert { get; set; }
    public bool? An { get; set; }
    public string Fehlermeldung { get; set; }
    public int? FehlerZeile { get; set; }
}
```

## Erwartete Anweisungs-Klassen

- `LampeAnweisung` (mit `Raum`, `Lampe`, `An` (bool))
- `DimmenAnweisung` (mit `Raum`, `Lampe`, `Helligkeit` (0-100))
- `WarteAnweisung` (mit `Sekunden`)
- `WennAnweisung` (mit `Sensor`, `Raum`, `Vergleich` (Enum), `Wert`, `Block`)
- `BlockAnweisung` (mit `List<Anweisung>`)

## Server-Logik

```csharp
async void NachrichtErhalten(SmartHomeMessage msg, Transfer<SmartHomeMessage> client)
{
    if (msg.Typ == MsgType.SkriptSenden)
    {
        try
        {
            var anweisungen = Parser.Parse(msg.Skript);
            // In eigenem Task ausfuehren damit Server nicht blockiert
            _ = Task.Run(async () =>
            {
                var ctx = new SmartHomeContext
                {
                    Raeume = _raeume,
                    AufStatusAenderung = beschreibung => AlleClientsBenachrichtigen()
                };

                foreach (var anw in anweisungen)
                    await anw.AusfuehrenAsync(ctx);
            });
        }
        catch (Exception ex)
        {
            client.Send(new SmartHomeMessage
            {
                Typ = MsgType.Fehler,
                Fehlermeldung = ex.Message,
                FehlerZeile = (ex as ParseException)?.Zeile
            });
        }
    }
}
```

## Sensor-Simulation am Server

```csharp
private DispatcherTimer _sensorTimer;
private Random _rnd = new();

_sensorTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
_sensorTimer.Tick += (s, e) =>
{
    foreach (var raum in _raeume)
        foreach (var sensor in raum.Sensoren)
        {
            // Kleine zufaellige Aenderung +-2
            sensor.Wert += _rnd.NextDouble() * 4 - 2;
            sensor.Wert = Math.Round(sensor.Wert, 1);
        }
    AlleClientsBenachrichtigen();
};
_sensorTimer.Start();
```

## Beispiel-Skript

`beispiel_skript.txt` enthält:

```
LAMPE Wohnzimmer Decke EIN
DIMMEN Wohnzimmer Decke 60
WARTE 3
WENN Helligkeit Wohnzimmer KLEINER 20 DANN {
    LAMPE Wohnzimmer Decke EIN
    DIMMEN Wohnzimmer Decke 100
    LAMPE Wohnzimmer Stehlampe EIN
}
WARTE 2
LAMPE Schlafzimmer Decke EIN
DIMMEN Schlafzimmer Decke 30
WENN Temperatur Schlafzimmer GROESSER 24 DANN {
    LAMPE Schlafzimmer Decke AUS
}
```

## ABNF-Tipp

```abnf
programm    = *anweisung
anweisung   = lampe / dimmen / warte / wenn
lampe       = "LAMPE" SP name SP name SP ("EIN" / "AUS")
dimmen      = "DIMMEN" SP name SP name SP zahl
warte       = "WARTE" SP zahl
wenn        = "WENN" SP name SP name SP vergleich SP zahl SP "DANN" SP block
vergleich   = "KLEINER" / "GLEICH" / "GROESSER"
block       = "{" *anweisung "}"
name        = 1*ALPHA
zahl        = 1*DIGIT
```

## TreeView-Tipp am Client

Live-Status der Räume:
```
Wohnzimmer
├── Decke (EIN, Helligkeit: 60)
├── Stehlampe (AUS)
├── Tischlampe (AUS)
├── Helligkeit: 47.3 lux
└── Temperatur: 22.1 C
Schlafzimmer
├── ...
```

Bei jedem `LampenStatus` oder `SensorWert` vom Server: TreeView neu aufbauen oder gezielt das betroffene Item aktualisieren.
