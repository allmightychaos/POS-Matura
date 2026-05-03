namespace SmartHome;

/// <summary>
/// Basisklasse fuer alle Anweisungen im SmartHome-Skript-Interpreter.
/// Jede konkrete Anweisung (LampeAnweisung, DimmenAnweisung, etc.)
/// erbt von dieser Klasse und implementiert AusfuehrenAsync.
/// </summary>
public abstract class Anweisung
{
    /// <summary>
    /// Zeilennummer im Quelltext (fuer Fehlermeldungen).
    /// </summary>
    public int Zeile { get; set; }

    /// <summary>
    /// Fuehrt diese Anweisung aus. Async damit WARTE die Server-GUI nicht blockiert.
    /// </summary>
    public abstract Task AusfuehrenAsync(SmartHomeContext ctx);
}
