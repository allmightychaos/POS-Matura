using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using TSP_Pathfinder; // Falls deine Knoten-Klasse hier liegt

namespace TSP_WPF
{
    // Hilfsklasse für eine Stadt / einen Knoten
    public class City
    {
        public double X { get; set; }
        public double Y { get; set; }
        public int Id { get; set; }

        // NEU: Speichert die direkten Verbindungen (Kanten) dieser Stadt
        public List<City> Neighbors { get; set; } = new List<City>();

        public City(int id, double x, double y)
        {
            Id = id;
            X = x;
            Y = y;
        }
    }

    public partial class MainWindow : Window
    {
        private List<City> cities = new List<City>();
        private int cityCounter = 0;
        private Random rnd = new Random();

        // Einstellungen für das Zeichnen
        private readonly double cityRadius = 6;
        private readonly Brush cityColor = Brushes.DarkBlue;
        private readonly Brush routeColor = Brushes.Crimson;

        public MainWindow()
        {
            InitializeComponent();
        }

        #region UI & Zeichen-Logik (Bereits fertig)

        private void MapCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Stadt an der geklickten Position hinzufügen
            Point clickPoint = e.GetPosition(MapCanvas);
            AddCity(clickPoint.X, clickPoint.Y);
        }

        private void BtnGenerateRandom_Click(object sender, RoutedEventArgs e)
        {
            // 15 zufällige Städte generieren
            double width = MapCanvas.ActualWidth == 0 ? 600 : MapCanvas.ActualWidth;
            double height = MapCanvas.ActualHeight == 0 ? 500 : MapCanvas.ActualHeight;

            for (int i = 0; i < 15; i++)
            {
                double x = rnd.NextDouble() * (width - 40) + 20;
                double y = rnd.NextDouble() * (height - 40) + 20;
                AddCity(x, y);
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            cities.Clear();
            cityCounter = 0;
            MapCanvas.Children.Clear();
        }

        // NEU: Button-Click für die Graph-Erstellung
        private void BtnGenerateGraph_Click(object sender, RoutedEventArgs e)
        {
            GenerateGraphConnections();
            DrawCities(); // Neu zeichnen, damit die Kanten sichtbar werden
        }

        private void AddCity(double x, double y)
        {
            City newCity = new City(cityCounter++, x, y);
            cities.Add(newCity);
            DrawCities();
        }

        private void GenerateGraphConnections()
        {
            // Zuerst alte Verbindungen löschen
            foreach (var city in cities)
            {
                city.Neighbors.Clear();
            }

            double maxDistance = 200.0; // Standard-Verbindungsradius

            // 1. Alle Städte verbinden, die nah genug beieinander sind
            for (int i = 0; i < cities.Count; i++)
            {
                for (int j = i + 1; j < cities.Count; j++)
                {
                    if (CalculateDistance(cities[i], cities[j]) <= maxDistance)
                    {
                        cities[i].Neighbors.Add(cities[j]);
                        cities[j].Neighbors.Add(cities[i]);
                    }
                }
            }

            // 2. Sicherheitsnetz: Isolierten Städten zwangsweise einen Weg bauen
            foreach (var city in cities)
            {
                // Wenn die Stadt keine Straßen hat...
                if (city.Neighbors.Count == 0 && cities.Count > 1)
                {
                    // ...finde die Stadt, die am nächsten ist (ignoriere sich selbst)
                    City nearest = cities
                        .Where(c => c != city)
                        .OrderBy(c => CalculateDistance(city, c))
                        .First();

                    // Baue eine Straße zwischen den beiden
                    city.Neighbors.Add(nearest);
                    nearest.Neighbors.Add(city);
                }
            }
        }

        private void DrawCities()
        {
            MapCanvas.Children.Clear();

            // 1. Zuerst das graue Wegenetz (den Graphen) zeichnen
            foreach (var city in cities)
            {
                foreach (var neighbor in city.Neighbors)
                {
                    // Um doppeltes Zeichnen zu vermeiden, nur zeichnen wenn Id kleiner ist
                    if (city.Id < neighbor.Id)
                    {
                        Line edge = new Line
                        {
                            X1 = city.X,
                            Y1 = city.Y,
                            X2 = neighbor.X,
                            Y2 = neighbor.Y,
                            Stroke = Brushes.LightGray,
                            StrokeThickness = 1
                        };
                        MapCanvas.Children.Add(edge);
                    }
                }
            }

            // 2. Dann die Städte (Punkte) darüber zeichnen
            foreach (var city in cities)
            {
                Ellipse ellipse = new Ellipse
                {
                    Width = cityRadius * 2,
                    Height = cityRadius * 2,
                    Fill = cityColor,
                    ToolTip = $"City {city.Id}"
                };

                Canvas.SetLeft(ellipse, city.X - cityRadius);
                Canvas.SetTop(ellipse, city.Y - cityRadius);
                MapCanvas.Children.Add(ellipse);
            }
        }

        private void DrawRoute(List<City> route)
        {
            // Erst alle Linien entfernen (Städte & Graph bleiben, da wir DrawCities neu aufrufen)
            DrawCities();

            if (route == null || route.Count < 2) return;

            for (int i = 0; i < route.Count - 1; i++)
            {
                Line line = new Line
                {
                    X1 = route[i].X,
                    Y1 = route[i].Y,
                    X2 = route[i + 1].X,
                    Y2 = route[i + 1].Y,
                    Stroke = routeColor,
                    StrokeThickness = 2
                };
                MapCanvas.Children.Add(line);
            }

            // Für TSP: Letzte Stadt mit der ersten verbinden um den Kreis zu schließen
            // Falls du für Dijkstra/A* nur einen Pfad von A nach B zeigst, 
            // setze hier eine kleine if-Bedingung, damit er den Kreis NICHT schließt.
            // Z.B. if (route.Count == cities.Count && route.Count > 2)
            if (route.Count > 2)
            {
                Line closingLine = new Line
                {
                    X1 = route.Last().X,
                    Y1 = route.Last().Y,
                    X2 = route.First().X,
                    Y2 = route.First().Y,
                    Stroke = routeColor,
                    StrokeThickness = 2,
                    StrokeDashArray = new DoubleCollection { 4, 2 } // Gestrichelt zur Unterscheidung
                };
                MapCanvas.Children.Add(closingLine);
            }
        }

        // Hilfsmethode: Berechnet die Euklidische Distanz (Luftlinie) zwischen zwei Städten
        private double CalculateDistance(City a, City b)
        {
            return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }

        #endregion

        #region Deine Algorithmen

        private void BtnNearestNeighbor_Click(object sender, RoutedEventArgs e)
        {
            if (cities.Count < 2) return;

            List<City> resultRoute = SolveNearestNeighbor(cities);
            DrawRoute(resultRoute);
        }

        private void BtnDijkstra_Click(object sender, RoutedEventArgs e)
        {
            if (cities.Count < 2) return;

            List<City> resultRoute = SolveDijkstra(cities);
            DrawRoute(resultRoute);
        }

        private void BtnAStar_Click(object sender, RoutedEventArgs e)
        {
            if (cities.Count < 2) return;

            List<City> resultRoute = SolveAStar(cities);
            DrawRoute(resultRoute);
        }



        // Dein implementierter Nearest Neighbor Algorithmus
        private List<City> SolveNearestNeighbor(List<City> allCities)
        {
            List<City> verbleibend = allCities.ToList();
            List<City> path = new List<City>();

            // Start-Stadt holen und aus verbleibend entfernen
            City startCity = verbleibend.First();

            path.Add(startCity);
            verbleibend.Remove(startCity);

            while (verbleibend.Count > 0)
            {
                // Letzte Stadt aus Pfad holen
                var last = path.Last();

                // Sortieren nach kürzester Distanz & nächste Stadt holen
                var naechste = verbleibend
                    .OrderBy(o => CalculateDistance(last, o))
                    .FirstOrDefault();

                // Zu Pfad hinzufügen & aus verbleibenden löschen
                path.Add(naechste);
                verbleibend.Remove(naechste);
            }

            return path; // Gib die sortierte Liste der Städte zurück
        }

        // TODO: Implementiere Dijkstra
        private List<City> SolveDijkstra(List<City> allCities)
        {
            // 0. Start und Ziel festlegen (Einfachkeitshalber mittels .First() und .Last())
            City startCity = allCities.First();
            City endCity = allCities.Last();
            

            // 1. Listen festlegen
            List<Knoten> toDoList = new List<Knoten>();
            HashSet<City> erledigtListe = new HashSet<City>();


            // 2. Startpunkt festlegen
            Knoten startKnoten = new Knoten()
            {
                Stadt = startCity,
                Vorgänger = null,
                GCost = 0
            };


            // 3. Auf die ToDo-Liste setzten 
            toDoList.Add(startKnoten);


            // 4. Hauptschleife
            while (toDoList.Count > 0)
            {
                // a. Besten Knoten wählen
                Knoten aktuell = toDoList.OrderBy(o => o.GCost).FirstOrDefault();


                // b. Ziel check (Liste `path` erstellen, und vom Ziel -> Anfang durchgehen, Reverse() um in die korrekte Reihenfolge)
                if (aktuell.Stadt == endCity)
                {
                    List<City> path = new List<City>();
                    Knoten pfadKnoten = aktuell;

                    while (pfadKnoten != null)
                    {
                        path.Add(pfadKnoten.Stadt);
                        pfadKnoten = pfadKnoten.Vorgänger;
                    }

                    path.Reverse();
                    return path;
                }


                // c. Knoten verarbeiten & von ToDo-Liste entfernen
                toDoList.Remove(aktuell);
                erledigtListe.Add(aktuell.Stadt);


                // d. Nachbarn untersuchen
                foreach (City nachbarStadt in aktuell.Stadt.Neighbors)
                {
                    if (erledigtListe.Contains(nachbarStadt)) continue;


                    // e. Neue Distanz errechnen (ggf. updaten)
                    double neueDistanz = aktuell.GCost + CalculateDistance(aktuell.Stadt, nachbarStadt);
                    
                    Knoten nachbarKnoten = toDoList.FirstOrDefault(f => f.Stadt == nachbarStadt);

                    // Fall A: Nachbar ist noch nicht auf der Todo-Liste
                    if (nachbarKnoten == null)
                    {
                        nachbarKnoten = new Knoten()
                        {
                            Stadt = nachbarStadt,
                            GCost = neueDistanz,
                            Vorgänger = aktuell
                        };
                        toDoList.Add(nachbarKnoten);
                    }

                    // Fall B: Nachbar ist bereits auf der Liste
                    else if (neueDistanz < nachbarKnoten.GCost)
                    {
                        nachbarKnoten.GCost = neueDistanz;
                        nachbarKnoten.Vorgänger = aktuell;
                    }
                }
            }


            return null;
        }

        // TODO: Implementiere A*
        private List<City> SolveAStar(List<City> allCities)
        {
            // 0. Start und Ziel festlegen (Einfachkeitshalber mittels .First() und .Last())
            City startCity = allCities.First();
            City endCity = allCities.Last();


            // 1. Listen festlegen
            List<Knoten> toDoList = new List<Knoten>();
            HashSet<City> erledigtListe = new HashSet<City>();


            // 2. Startpunkt festlegen
            Knoten startKnoten = new Knoten()
            {
                Stadt = startCity,
                Vorgänger = null,
                GCost = 0,
                HCost = CalculateDistance(startCity, endCity)
            };


            // 3. Auf die ToDo-Liste setzten 
            toDoList.Add(startKnoten);


            // 4. Hauptschleife
            while (toDoList.Count > 0)
            {
                // a. Besten Knoten wählen
                Knoten aktuell = toDoList.OrderBy(o => o.FCost).FirstOrDefault();


                // b. Ziel check (Liste `path` erstellen, und vom Ziel -> Anfang durchgehen, Reverse() um in die korrekte Reihenfolge)
                if (aktuell.Stadt == endCity)
                {
                    List<City> path = new List<City>();
                    Knoten pfadKnoten = aktuell;

                    while (pfadKnoten != null)
                    {
                        path.Add(pfadKnoten.Stadt);
                        pfadKnoten = pfadKnoten.Vorgänger;
                    }

                    path.Reverse();
                    return path;
                }


                // c. Knoten verarbeiten & von ToDo-Liste entfernen
                toDoList.Remove(aktuell);
                erledigtListe.Add(aktuell.Stadt);


                // d. Nachbarn untersuchen
                foreach (City nachbarStadt in aktuell.Stadt.Neighbors)
                {
                    if (erledigtListe.Contains(nachbarStadt)) continue;


                    // e. Neue Distanz errechnen (ggf. updaten)
                    double neueDistanz = aktuell.GCost + CalculateDistance(aktuell.Stadt, nachbarStadt);

                    Knoten nachbarKnoten = toDoList.FirstOrDefault(f => f.Stadt == nachbarStadt);

                    // Fall A: Nachbar ist noch nicht auf der Todo-Liste
                    if (nachbarKnoten == null)
                    {
                        nachbarKnoten = new Knoten()
                        {
                            Stadt = nachbarStadt,
                            GCost = neueDistanz,
                            HCost = CalculateDistance(nachbarStadt, endCity),
                            Vorgänger = aktuell
                        };
                        toDoList.Add(nachbarKnoten);
                    }

                    // Fall B: Nachbar ist bereits auf der Liste
                    else if (neueDistanz < nachbarKnoten.GCost)
                    {
                        nachbarKnoten.GCost = neueDistanz;
                        nachbarKnoten.Vorgänger = aktuell;
                    }
                }
            }


            return null;
        }

        #endregion
    }
}