using System.Windows.Media;

namespace Schildkroete;

/// <summary>
/// Haelt den aktuellen Zustand der Schildkroete waehrend der Ausfuehrung
/// des Programms. Wird allen Anweisungen uebergeben.
/// </summary>
public class SchildkroeteContext
{
    /// <summary>
    /// X-Koordinate der Schildkroete am Zeichenbrett (in Pixel).
    /// </summary>
    public double X { get; set; } = 400;

    /// <summary>
    /// Y-Koordinate der Schildkroete am Zeichenbrett (in Pixel).
    /// </summary>
    public double Y { get; set; } = 300;

    /// <summary>
    /// Blickrichtung in Grad. 0 = nach rechts, 90 = nach unten,
    /// 180 = nach links, 270 = nach oben. (Mathematisch positiv im
    /// Uhrzeigersinn, weil Y-Achse in WPF nach unten geht.)
    /// </summary>
    public double WinkelGrad { get; set; } = 0;

    /// <summary>
    /// True = Stift unten (zeichnet bei Bewegung).
    /// False = Stift oben (bewegt sich ohne zu zeichnen).
    /// </summary>
    public bool StiftUnten { get; set; } = false;

    /// <summary>
    /// Aktuelle Stiftfarbe.
    /// </summary>
    public Brush AktuelleFarbe { get; set; } = Brushes.Black;

    /// <summary>
    /// Referenz auf das ZeichenBrett, auf dem gezeichnet wird.
    /// Wird vom Hauptprogramm vor dem Ausfuehren gesetzt.
    /// </summary>
    public ZeichenBrett Brett { get; set; }

    /// <summary>
    /// Pause in Millisekunden zwischen den Anweisungen.
    /// Wird vom Hauptprogramm gesetzt (Default 100 ms laut Aufgabe).
    /// </summary>
    public int PauseMs { get; set; } = 100;

    /// <summary>
    /// Hilfsmethode: wandelt einen Farbnamen aus dem Skript
    /// (ROT, GRUEN, BLAU, GELB, SCHWARZ, WEISS, ORANGE, VIOLETT)
    /// in einen Brush um.
    /// Wirft eine ArgumentException bei unbekannter Farbe.
    /// </summary>
    public static Brush BrushVomNamen(string name)
    {
        return name.ToUpper() switch
        {
            "ROT" => Brushes.Red,
            "GRUEN" => Brushes.Green,
            "BLAU" => Brushes.Blue,
            "GELB" => Brushes.Yellow,
            "SCHWARZ" => Brushes.Black,
            "WEISS" => Brushes.White,
            "ORANGE" => Brushes.Orange,
            "VIOLETT" => Brushes.Violet,
            _ => throw new ArgumentException($"Unbekannte Farbe: {name}")
        };
    }
}
