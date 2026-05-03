namespace Schildkroete;

/// <summary>
/// Basisklasse fuer alle Anweisungen im Schildkroeten-Interpreter.
/// Jede konkrete Anweisung (VorwaertsAnweisung, RechtsAnweisung, etc.)
/// erbt von dieser Klasse und implementiert AusfuehrenAsync.
/// </summary>
public abstract class Anweisung
{
    /// <summary>
    /// Zeilennummer im Quelltext (fuer Fehlermeldungen)
    /// </summary>
    public int Zeile { get; set; }

    /// <summary>
    /// Fuehrt diese Anweisung aus. Async, damit Pausen zwischen
    /// den Schritten moeglich sind, ohne die GUI zu blockieren.
    /// </summary>
    public abstract Task AusfuehrenAsync(SchildkroeteContext ctx);
}
