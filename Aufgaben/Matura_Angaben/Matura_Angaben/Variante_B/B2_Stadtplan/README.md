# B2 Stadtplan-Routenplaner - Setup

## Was wird hier gemacht?

WPF-Programm mit SQLite-Datenbank (linq2db). Lädt einen Stadtgraphen (basierend auf Wiener U-Bahn/Stations), zeigt ihn am Canvas an, berechnet mit Dijkstra die kürzeste Route oder schnellste Fahrzeit zwischen zwei Knoten.

## Angabe-Dateien

| Datei | Was ist drin? | Wo hinkopieren? |
| :--- | :--- | :--- |
| `knoten.csv` | 17 Wiener Stationen mit Geo-Koordinaten | Projektroot, "In Ausgabeverzeichnis kopieren" |
| `kanten.csv` | 48 Kanten (24 bidirektionale Verbindungen) mit Länge und MaxKmh | Projektroot, "In Ausgabeverzeichnis kopieren" |

CSV-Format: UTF-8, Semikolon-getrennt, Header-Zeile.

## Format

**knoten.csv**
```
Id;Name;Longitude;Latitude
1;Hauptbahnhof;16.3754;48.1857
...
```

**kanten.csv** (gerichtet, deshalb für jede Verbindung 2 Einträge)
```
Id;VonKnotenId;NachKnotenId;LaengeMeter;MaxKmh
1;1;2;1700;50
2;2;1;1700;50
...
```

## NuGet-Pakete

- `linq2db`
- `linq2db.SQLite`
- `Microsoft.Data.Sqlite`

## Datenmodell

```csharp
[Table("Knoten")]
public class Knoten
{
    [Column, PrimaryKey] public int Id { get; set; }
    [Column] public string Name { get; set; }
    [Column] public double Longitude { get; set; }
    [Column] public double Latitude { get; set; }
}

[Table("Kante")]
public class Kante
{
    [Column, PrimaryKey, Identity] public int Id { get; set; }
    [Column] public int VonKnotenId { get; set; }
    [Column] public int NachKnotenId { get; set; }
    [Column] public double LaengeMeter { get; set; }
    [Column] public double MaxKmh { get; set; }
}
```

## Koordinaten-Umrechnung

Die Wien-Geo-Koordinaten liegen ungefähr in:

- Longitude: 16.31 bis 16.42 (Breite ca. 0.11 Grad)
- Latitude: 48.17 bis 48.26 (Höhe ca. 0.09 Grad)

Umrechnung auf Canvas-Pixel:

```csharp
private const double LonMin = 16.30, LonMax = 16.42;
private const double LatMin = 48.17, LatMax = 48.26;

private (double X, double Y) LonLatZuCanvas(double lon, double lat)
{
    double x = (lon - LonMin) / (LonMax - LonMin) * StadtCanvas.ActualWidth;
    // Y invertieren weil Canvas oben-links Ursprung hat, Geo unten-links
    double y = (1 - (lat - LatMin) / (LatMax - LatMin)) * StadtCanvas.ActualHeight;
    return (x, y);
}
```

## Dijkstra-Pseudocode

```csharp
public static List<int> Dijkstra(int startId, int zielId, List<Knoten> knoten, List<Kante> kanten,
                                  Func<Kante, double> gewicht)
{
    var distanz = knoten.ToDictionary(k => k.Id, _ => double.MaxValue);
    var vorgaenger = new Dictionary<int, int?>();
    distanz[startId] = 0;

    var queue = new PriorityQueue<int, double>();
    queue.Enqueue(startId, 0);

    while (queue.TryDequeue(out int aktuell, out _))
    {
        if (aktuell == zielId) break;

        foreach (var k in kanten.Where(k => k.VonKnotenId == aktuell))
        {
            double neueDistanz = distanz[aktuell] + gewicht(k);
            if (neueDistanz < distanz[k.NachKnotenId])
            {
                distanz[k.NachKnotenId] = neueDistanz;
                vorgaenger[k.NachKnotenId] = aktuell;
                queue.Enqueue(k.NachKnotenId, neueDistanz);
            }
        }
    }

    // Pfad zurueck rekonstruieren
    var pfad = new List<int>();
    int? cur = zielId;
    while (cur.HasValue)
    {
        pfad.Insert(0, cur.Value);
        vorgaenger.TryGetValue(cur.Value, out int? prev);
        cur = prev;
    }
    return pfad.First() == startId ? pfad : new List<int>(); // leer wenn nicht erreichbar
}
```

## Optimierungsziel umschalten

Dem Dijkstra einen `Func<Kante, double>` übergeben:

- Distanz: `k => k.LaengeMeter`
- Fahrzeit: `k => k.LaengeMeter / (k.MaxKmh / 3.6)` (in Sekunden)

## Klick-Handling am Canvas

```csharp
private void StadtCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
{
    var pos = e.GetPosition(StadtCanvas);
    var naechster = FindeNaechstenKnoten(pos.X, pos.Y);
    if (naechster != null) StartCombo.SelectedItem = naechster;
}

private void StadtCanvas_MouseRightButtonDown(...)  // analog für Ziel
```
