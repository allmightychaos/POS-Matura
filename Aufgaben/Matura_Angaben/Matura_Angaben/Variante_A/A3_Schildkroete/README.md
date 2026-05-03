# A3 Schildkröten-Grafik - Setup

## Was wird hier gemacht?

WPF-Programm mit eigener UserControl `ZeichenBrett` (Canvas-basiert) und einem Interpreter für eine Schildkröten-Sprache (LOGO-ähnlich). Programme werden aus Textdateien geladen, geparst, dann auf dem Zeichenbrett ausgeführt mit 100 ms Pause zwischen den Schritten.

## Angabe-Dateien

| Datei | Was ist drin? | Wo hinkopieren? |
| :--- | :--- | :--- |
| `Anweisung.cs` | Abstrakte Basisklasse für alle Anweisungen | Projektroot, "Hinzufügen > Vorhandenes Element" |
| `SchildkroeteContext.cs` | State-Holder (Position, Winkel, Stift, Farbe) | Projektroot, "Hinzufügen > Vorhandenes Element" |
| `programm1.txt` | Quadrat (Grundtest) | Projektroot, "In Ausgabeverzeichnis kopieren" |
| `programm2.txt` | Stern + Kreis (Verschachtelung) | Projektroot, "In Ausgabeverzeichnis kopieren" |
| `Fehler1.txt` | Fehlende öffnende Klammer | Projektroot, "In Ausgabeverzeichnis kopieren" |
| `Fehler2.txt` | Ungültige Farbe `LILA` | Projektroot, "In Ausgabeverzeichnis kopieren" |

## Namespace anpassen

Beide C#-Dateien benutzen den Namespace `Schildkroete`. Wenn dein Projekt einen anderen Namespace hat (z.B. `Matura_A3`), entweder:

- Den Namespace in den beiden Dateien anpassen, ODER
- In deinem Code `using Schildkroete;` schreiben (einfacher)

## Was du selbst schreiben musst

### Konkrete Anweisungs-Klassen (alle erben von `Anweisung`)

- `VorwaertsAnweisung` (mit Property `Pixel`)
- `RueckwaertsAnweisung` (mit Property `Pixel`)
- `LinksAnweisung` (mit Property `Grad`)
- `RechtsAnweisung` (mit Property `Grad`)
- `StiftSenkenAnweisung`
- `StiftHebenAnweisung`
- `FarbeAnweisung` (mit Property `Farbname`)
- `WiederholeAnweisung` (mit Properties `Anzahl` und `Block`)
- `BlockAnweisung` (mit `List<Anweisung>`)

### Parser

Liest die Textdatei zeilenweise und baut die Baumstruktur auf. Bei verschachtelten `{ }` brauchst du entweder einen rekursiven Parser oder einen Cursor-basierten Parser.

### ZeichenBrett (eigene UserControl)

```csharp
public partial class ZeichenBrett : UserControl
{
    private Canvas _canvas;
    private Polygon _schildkroete; // Dreieck
    private RotateTransform _rotation;

    public ZeichenBrett()
    {
        InitializeComponent();
        // Schildkröten-Dreieck initialisieren
    }

    public void LinieZeichnen(double x1, double y1, double x2, double y2, Brush farbe)
    {
        var line = new Line { X1 = x1, Y1 = y1, X2 = x2, Y2 = y2,
                              Stroke = farbe, StrokeThickness = 2 };
        _canvas.Children.Add(line);
    }

    public void SchildkroeteSetzen(double x, double y, double winkelGrad)
    {
        Canvas.SetLeft(_schildkroete, x - 8);
        Canvas.SetTop(_schildkroete, y - 8);
        _rotation.Angle = winkelGrad;
    }

    public void Loeschen()
    {
        // Alle Linien entfernen, Schildkröte stehen lassen
        var zuEntfernen = _canvas.Children.OfType<Line>().ToList();
        foreach (var l in zuEntfernen) _canvas.Children.Remove(l);
    }
}
```

### Programmstruktur (Beispiel)

```csharp
// Programm laden
string text = File.ReadAllText(filePfad);

// Parsen
List<Anweisung> programm = Parser.Parse(text); // throws bei Syntaxfehler

// Ausführen
var ctx = new SchildkroeteContext { Brett = MeinBrett, PauseMs = 100 };
foreach (var anw in programm)
{
    await anw.AusfuehrenAsync(ctx);
}
```

## Bewegung umrechnen (Polarkoordinaten)

```csharp
double rad = ctx.WinkelGrad * Math.PI / 180.0;
double neuX = ctx.X + Math.Cos(rad) * pixel;
double neuY = ctx.Y + Math.Sin(rad) * pixel;

if (ctx.StiftUnten)
    ctx.Brett.LinieZeichnen(ctx.X, ctx.Y, neuX, neuY, ctx.AktuelleFarbe);

ctx.X = neuX;
ctx.Y = neuY;
ctx.Brett.SchildkroeteSetzen(ctx.X, ctx.Y, ctx.WinkelGrad);
await Task.Delay(ctx.PauseMs);
```

## Erwartete Ausgaben

- `programm1.txt` zeichnet ein 100x100 Pixel großes Quadrat
- `programm2.txt` zeichnet einen roten 5-zackigen Stern aus 3-Eck-Wiederholung, dann mit blauem Stift einen 36-eckigen Kreis daneben
- `Fehler1.txt` muss eine Fehlermeldung mit Zeilennummer 2 (oder 3) ausgeben (öffnende Klammer fehlt nach `WIEDERHOLE 4`)
- `Fehler2.txt` muss eine Fehlermeldung mit Zeilennummer 1 ausgeben (LILA ist keine gültige Farbe)

## ABNF-Tipp

Die Grammatik wird einfacher wenn du das Top-Level-Element als Liste von Anweisungen modellierst:

```abnf
programm   = *anweisung
anweisung  = bewegung / drehung / stift / farbe / wiederhole / block
bewegung   = ("VORWAERTS" / "RUECKWAERTS") SP zahl
drehung    = ("LINKS" / "RECHTS") SP zahl
stift      = "STIFT" SP ("HEBEN" / "SENKEN")
farbe      = "FARBE" SP farbname
farbname   = "ROT" / "GRUEN" / "BLAU" / "GELB" / "SCHWARZ" / "WEISS" / "ORANGE" / "VIOLETT"
wiederhole = "WIEDERHOLE" SP zahl SP block
block      = "{" *anweisung "}"
zahl       = 1*DIGIT
```
