# Walwunder Aufgabenstellung

## GUI

- [X] Skalierbar -> Grid
- [X] Menüpunkt: Neues Waldwunder anlegen
- [X] **Dialog anzeigen**:
  - Alle Daten des Waldwunder eingeben
  - Beliebige Anzahl an Bilder für das Walwunder auswählen
  - Dateiname (ohne Pfad) der ausg. Bilder in einer ListBox anzeigen
    - Bilder wieder entfernen
  - Auf Button Registrieren in dem Dialog gedrückt -> alle Daten und Bilder speichern
- [X] Vorhandenen Walwunder nach Stichwort, Art oder Ort abfragen können
  - **Stichwort**: alle Waldwunder die das `Stichwort` im Titel oder Beschreibung haben
  - **Art**: alle dieser Art
  - **Ort**: mit Geo-Koordinaten in Long & Lat (Abweichung: ± 0,5)
- [X] gefunden Walwunder in einer ListBox (Titel und Beschreibung) neben der Karte anzeigen
  - [X] und in der Karte als Markierung 
- [X] wird in der Karte auf eine Markierung geklickt, soll das enstprechende Walwunder in der Liste selektiert werden
- [X] wurde ein Waldwunder selektiert, soll über einen Button Anzeigen die Bilder des Waldwunders und die Daten ein einem Dialog angezeigt werden 

## Bilder

- [X] Als Dateien in einem Ordner speichern
- [X] Wenn Bilder den selben Namen haben -> umbenennen mit fortlaufender Nummer


## Daten

- [X] In einer SQLite DB speichern
- [X] Von einer SQLite DB laden

### Feldbeschreibung Walwunder

```md
Feldname - Feldbeschreibung - Typ - Bemerkung
id - Waldwunder-Nr. - int - z.B. 32
name - Name - String - z.B. Mangrovenwald
description - Beschreibung - String - z.B. Man wähnt sich beinahe im tropischen Mangrovenwald, auf diesen ... 
province - Bundesland - Enum - z.B. Niederösterreich
latitude - Latitude - float - z.B. 48,131836
longitude - Longitude - float - z.B. 16,687295
type - Art - String - z.B. Wald
votes - Stimmen (wird nicht verwendet) - int - z.B. 14
```

### Feldbeschreibung Bilder

```md
Feldname - Feldbeschreibung - Typ - Bemerkung
id - Bild-Nr. - int - z.B. 215
name - Dateiname - String - z.B. Mangroven.png
wonder - Waldwunder - int - z.B. 32
```