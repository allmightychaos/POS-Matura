# A2 Bibliotheks-Verwaltung - Setup

## Was wird hier gemacht?

Server-Client-Programm mit SQLite-Datenbank für eine Bücherei. Der Server hält die DB, mehrere Clients können verbinden und suchen, ausleihen, etc. Genres sind hierarchisch (TreeView). Netzwerk über `Transfer<T>` mit einer einzigen `BibliothekMessage`-Klasse plus `MsgType`-Enum.

## Angabe-Dateien

| Datei | Was ist drin? | Wo hinkopieren? |
| :--- | :--- | :--- |
| `autoren.csv` | 20 Autoren (Id;Name) | Server-Projektroot, "In Ausgabeverzeichnis kopieren" |
| `genres.csv` | 17 Genres in 3-Ebenen-Hierarchie | Server-Projektroot, "In Ausgabeverzeichnis kopieren" |
| `buecher.csv` | 40 Bücher | Server-Projektroot, "In Ausgabeverzeichnis kopieren" |

Die CSVs sind UTF-8 mit Semikolon als Trenner und einer Header-Zeile.

## Format der CSVs

**autoren.csv**
```
Id;Name
1;Stephen King
...
```

**genres.csv** (`ParentId` ist leer für Root-Kategorien)
```
Id;Name;ParentId
1;Roman;
3;Krimi;1
11;Thriller;3
...
```

Hier wäre `Krimi` ein Subgenre von `Roman`, und `Thriller` ein Sub-Subgenre von `Krimi`.

**buecher.csv**
```
Id;Titel;AutorId;GenreId;Verfuegbar
1;Es;1;11;true
...
```

## NuGet-Pakete

Beide Projekte (Server und Client) brauchen:

- `linq2db`
- `linq2db.SQLite`
- `Microsoft.Data.Sqlite`

Im Server zusätzlich nichts (TCP ist in `System.Net.Sockets`).

## Transfer_2026.cs

Die Datei `Transfer_2026.cs` (du hast sie schon) gehört in **beide** Projekte (Server und Client). Einmal bei Server kopieren, einmal bei Client. Namespace ist `Network`.

## DB-Setup mit linq2db

```csharp
public class BibliothekDb : DataConnection
{
    public BibliothekDb(DataOptions<BibliothekDb> options) : base(options.Options) { }
    public ITable<Buch> Buecher => this.GetTable<Buch>();
    public ITable<Autor> Autoren => this.GetTable<Autor>();
    public ITable<Genre> Genres => this.GetTable<Genre>();
}
```

DataOptions in Program.cs / App.xaml.cs:
```csharp
var options = new DataOptions().UseSQLite("Data Source=bibliothek.db");
using var db = new BibliothekDb(new DataOptions<BibliothekDb>(options));
db.CreateTable<Buch>(tableOptions: TableOptions.CreateIfNotExists);
// ... gleiches fuer Autor und Genre
```

## CSV einlesen

```csharp
foreach (var line in File.ReadAllLines("autoren.csv").Skip(1)) // Header skippen
{
    var teile = line.Split(';');
    db.Insert(new Autor { Id = int.Parse(teile[0]), Name = teile[1] });
}
```

## TreeView aus rekursiver Hierarchie

LINQ-Idee:
```csharp
var alleGenres = db.Genres.ToList();
var roots = alleGenres.Where(g => g.ParentId == null);
foreach (var root in roots)
{
    var node = new TreeViewItem { Header = root.Name };
    BaueKinder(node, root, alleGenres);
    GenreTree.Items.Add(node);
}

void BaueKinder(TreeViewItem parent, Genre g, List<Genre> alle)
{
    foreach (var kind in alle.Where(x => x.ParentId == g.Id))
    {
        var node = new TreeViewItem { Header = kind.Name, Tag = kind };
        parent.Items.Add(node);
        BaueKinder(node, kind, alle);
    }
}
```

## BibliothekMessage-Klasse

```csharp
public class BibliothekMessage
{
    public enum MsgType { Suchen, Suchergebnis, Ausleihen, AusleiheBestaetigung, NeuesBuch, Fehler }
    public MsgType Typ { get; set; }
    public string Suchbegriff { get; set; }
    public int? GenreId { get; set; }
    public int? BuchId { get; set; }
    public List<Buch> Suchergebnisse { get; set; }
    public bool Erfolgreich { get; set; }
    public string Fehlermeldung { get; set; }
}
```

`Transfer<T>` benoetigt **eine** Klasse fuer alle Nachrichtentypen, deshalb das `MsgType`-Enum als Diskriminator.

## Suche mit Untergenre-Inklusion

Wenn der User auf "Roman" klickt, sollen auch Bücher der Untergenres "Krimi", "Thriller", "Detektivroman", "Fantasy", etc. in den Ergebnissen sein. Lösung: rekursiv alle Untergenre-IDs sammeln, dann mit `Where(b => unterIds.Contains(b.GenreId))` filtern.
