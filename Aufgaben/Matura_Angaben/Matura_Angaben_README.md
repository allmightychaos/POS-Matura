# Matura-Angaben - Setup & Übersicht

Dieses Verzeichnis enthält alle Angabe-Dateien für die zwei Übungs-Maturen sowie eine zentrale Aufgabenstellung.

## Verzeichnisstruktur

```
Matura_Angaben/
├── README.md                         (diese Datei - Setup & Übersicht)
├── Matura_Aufgaben.md                (alle 6 Aufgabenstellungen + Punktesystem)
├── Variante_A/
│   ├── A1_Skigebiet/
│   │   ├── README.md                 (Setup-Anleitung A1)
│   │   └── skigebiet.xml             (Skigebiets-Daten)
│   ├── A2_Bibliothek/
│   │   ├── README.md
│   │   ├── autoren.csv
│   │   ├── genres.csv
│   │   └── buecher.csv
│   └── A3_Schildkroete/
│       ├── README.md
│       ├── Anweisung.cs              (Basisklasse für Interpreter)
│       ├── SchildkroeteContext.cs    (State-Holder)
│       ├── programm1.txt             (Quadrat - testet Grundfunktionen)
│       ├── programm2.txt             (Stern - testet Verschachtelung)
│       ├── Fehler1.txt               (fehlende Klammer)
│       └── Fehler2.txt               (ungültige Farbe)
└── Variante_B/
    ├── B1_Werkstatt/
    │   └── README.md                 (keine Daten-Dateien nötig)
    ├── B2_Stadtplan/
    │   ├── README.md
    │   ├── knoten.csv
    │   └── kanten.csv
    └── B3_SmartHome/
        ├── README.md
        ├── Anweisung.cs
        ├── SmartHomeContext.cs
        ├── raeume.xml
        └── beispiel_skript.txt
```

## Workflow pro Beispiel

Für jedes Beispiel erstellst du ein neues WPF-Projekt in Visual Studio:

1. **Visual Studio öffnen** > Neues Projekt > "WPF-Anwendung" (.NET 9)
2. Projektname z.B. `Matura_A1_Skigebiet`
3. Ablageort frei wählbar (z.B. `C:\POS-Matura\`)

Anschließend die Angabe-Dateien wie unten beschrieben einbinden.

## Wie kopiere ich die Angabe-Dateien?

Es gibt zwei Arten von Angabe-Dateien, die unterschiedlich behandelt werden:

### 1. Daten-Dateien (`.xml`, `.csv`, `.txt`)

Diese werden zur Laufzeit gelesen und müssen ins Ausgabeverzeichnis (Build-Ordner) kopiert werden.

**Schritt 1**: Datei per Windows Explorer in den **Projektroot** kopieren (das Verzeichnis mit der `.csproj`).

**Schritt 2**: In Visual Studio im Projektmappen-Explorer rechte Maustaste auf das Projekt > **Hinzufügen > Vorhandenes Element** > Datei auswählen.

**Schritt 3**: Die Datei im Projektmappen-Explorer markieren > F4 (Eigenschaften) > **"In Ausgabeverzeichnis kopieren"** auf **"Bei neueren kopieren"** stellen.

**Im Code zugreifen**:
```csharp
string pfad = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "skigebiet.xml");
```
Oder einfacher (relativ zum aktuellen Verzeichnis):
```csharp
string pfad = "skigebiet.xml";
```

### 2. C#-Dateien (`Anweisung.cs`, `SchildkroeteContext.cs`, etc.)

Diese sind Bestandteil deines Codes und werden in das Projekt aufgenommen.

**Schritt 1**: Datei per Windows Explorer in den **Projektroot** kopieren.

**Schritt 2**: In Visual Studio rechte Maustaste auf das Projekt > **Hinzufügen > Vorhandenes Element** > Datei auswählen.

**Schritt 3**: Im Code mit `using` referenzieren falls nötig (Namespace beachten - die Dateien benutzen den Namespace, der in der jeweiligen Datei steht).

## NuGet-Pakete pro Beispiel

| Beispiel | Pakete |
| :--- | :--- |
| A1 Skigebiet | (keine besonderen) |
| A2 Bibliothek | `linq2db`, `linq2db.SQLite`, `Microsoft.Data.Sqlite` |
| A3 Schildkröte | (keine) |
| B1 Werkstatt | (keine) |
| B2 Stadtplan | `linq2db`, `linq2db.SQLite`, `Microsoft.Data.Sqlite` |
| B3 SmartHome | (keine) |

NuGet-Pakete installieren: rechte Maustaste auf Projekt > **NuGet-Pakete verwalten** > Suche nach Paketnamen > Installieren.

## Transfer_2026.cs

Für die Beispiele **A2** und **B3** brauchst du die Datei `Transfer_2026.cs`. Diese liegt **nicht** im Angabe-Ordner (du hast sie selbst). Kopiere sie wie eine normale C#-Datei in dein Projekt:

```
Projektroot/
├── App.xaml
├── MainWindow.xaml
├── Transfer_2026.cs        <- hierhin kopieren
└── ...
```

Der Namespace ist `Network`, also im Code:
```csharp
using Network;

var transfer = new Transfer<BibliothekMessage>(client);
```

## Wenn du nur ein einzelnes Beispiel üben willst

Du kannst die Beispiele unabhängig voneinander bearbeiten. Im jeweiligen Unterordner liegt eine spezifische `README.md` mit den Details zu diesem Beispiel.

## Zeitplanung

Für die echte Matura: 4 bis 4,5 Stunden für 3 Beispiele. Empfohlene Aufteilung:

- 0:00 - 1:30 - Beispiel 1 (sicherstes, oft DB oder Multithreading)
- 1:30 - 3:00 - Beispiel 2 (Interpreter, da Teilpunkte schon ohne lauffähigen Parser)
- 3:00 - 4:00 - Beispiel 3 (Risiko, eventuell nicht ganz fertig)
- 4:00 - 4:30 - Polish, Bugfixes, Pflichtteile finalisieren

Bei Zeitnot: lieber ein Beispiel komplett liegen lassen und in den anderen alle Punkte holen, statt überall nur 50%.

## Wenn du Fragen hast

Frag Claude. Lass dich aber nicht durchcoden, sondern nur auf Sprünge helfen, sonst lernst du nichts.
