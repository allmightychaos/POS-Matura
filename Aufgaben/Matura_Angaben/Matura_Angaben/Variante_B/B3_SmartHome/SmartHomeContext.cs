namespace SmartHome;

/// <summary>
/// Haelt den aktuellen Zustand des Smart-Home-Systems waehrend der Skript-Ausfuehrung.
/// Wird allen Anweisungen uebergeben.
/// </summary>
public class SmartHomeContext
{
    /// <summary>
    /// Alle Raeume des Smart-Home-Systems.
    /// </summary>
    public List<Raum> Raeume { get; set; } = new();

    /// <summary>
    /// Callback fuer Status-Aenderungen. Wird vom Server gesetzt um
    /// alle verbundenen Clients ueber Aenderungen zu benachrichtigen.
    /// Parameter: Beschreibung der Aenderung als String.
    /// </summary>
    public Action<string> AufStatusAenderung { get; set; }

    /// <summary>
    /// Sucht eine Lampe in einem bestimmten Raum.
    /// Wirft KeyNotFoundException wenn Raum oder Lampe nicht existiert.
    /// </summary>
    public Lampe FindeLampe(string raumName, string lampenName)
    {
        var raum = Raeume.FirstOrDefault(r => r.Name.Equals(raumName, StringComparison.OrdinalIgnoreCase))
                   ?? throw new KeyNotFoundException($"Raum nicht gefunden: {raumName}");

        return raum.Lampen.FirstOrDefault(l => l.Name.Equals(lampenName, StringComparison.OrdinalIgnoreCase))
               ?? throw new KeyNotFoundException($"Lampe '{lampenName}' nicht gefunden in Raum '{raumName}'");
    }

    /// <summary>
    /// Sucht einen Sensor in einem bestimmten Raum.
    /// Wirft KeyNotFoundException wenn Raum oder Sensor nicht existiert.
    /// </summary>
    public Sensor FindeSensor(string raumName, string sensorName)
    {
        var raum = Raeume.FirstOrDefault(r => r.Name.Equals(raumName, StringComparison.OrdinalIgnoreCase))
                   ?? throw new KeyNotFoundException($"Raum nicht gefunden: {raumName}");

        return raum.Sensoren.FirstOrDefault(s => s.Name.Equals(sensorName, StringComparison.OrdinalIgnoreCase))
               ?? throw new KeyNotFoundException($"Sensor '{sensorName}' nicht gefunden in Raum '{raumName}'");
    }
}

public class Raum
{
    public string Name { get; set; }
    public List<Lampe> Lampen { get; set; } = new();
    public List<Sensor> Sensoren { get; set; } = new();
}

public class Lampe
{
    public string Name { get; set; }
    public bool An { get; set; }
    public int Helligkeit { get; set; } // 0 bis 100
}

public class Sensor
{
    public string Name { get; set; }   // z.B. "Helligkeit" oder "Temperatur"
    public double Wert { get; set; }
    public string Einheit { get; set; } // z.B. "lux" oder "C"
}
