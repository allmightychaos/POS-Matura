using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleThreadApp
{
    public class Primzahlenberechner
    {
        static object lockObj = new object();
        static List<int> allPrims = new List<int>();

        static void OldMain(string[] args)
        {
            int threadsCount = int.Parse(args[0]);  // Anzahl an Threads
            int primMax = int.Parse(args[1]);       // Maximale Zahl zum überprüfen
            int interval = primMax / threadsCount;  // Interval der Threads (10.000 / 4 => 2.000) 

            // Größter gemeinsamer Teiler (√primMax) => bspw. primMax = 1.600.000 -> √1.600.000 -> ~1.265
            int maxTeiler = (int)Math.Ceiling(Math.Sqrt(primMax)); 

            // Lists
            List<Thread> threads = new List<Thread>();  // Liste mit den Threads
            List<int> primTeiler = new List<int>();     // Liste der maxTeiler Primzahlen
            List<int> threadsInterval = new List<int>() { 0 }; // Initialisiere Threads-Interval Liste (mit 0 auf index 0) 

            // Measure elapsed time
            Stopwatch watch = new Stopwatch();
            watch.Start();


            // PrimTeiler finden
            Prim(5, maxTeiler, primTeiler);


            // Threads erstellen
            for (int i = 0; i < threadsCount; i++)
            {
                int start = maxTeiler  + 1;
                if (i > 0)
                {
                    start = threadsInterval[i] + 1;
                }

                // [0, 2000, 4000, ...] => int end
                threadsInterval.Add(threadsInterval[i] + interval);

                int end = threadsInterval[i + 1];

                // Create new Thread
                Thread t = new Thread(() => Prim(start, end, allPrims, primTeiler));
                threads.Add(t);

                t.Start();
            }

            foreach (Thread t in threads)
            {
                t.Join();
            }

            foreach (var item in primTeiler)
            {
                allPrims.Add(item);
            }

            allPrims.Sort();

            watch.Stop();

            // Outputs
            Console.WriteLine("Es wurden {0} Primzahlen gefunden", allPrims.Count);
            Console.WriteLine("Die höchste gefundene Primzahl ist {0}", allPrims[allPrims.Count - 1]);
            Console.WriteLine("Die Laufzeit betrug {0:F0} Millisekungen", watch.ElapsedMilliseconds);
            // Console.WriteLine("Es wurden {0} Vergleiche durchgeführt", tests);
        }

        private static void Prim(int start, int end, List<int> liste, List<int> teiler = null)
        {
            // start = Anfangszahl, end = Endzahl, liste = Liste, in die die Primzahlen eingefügt werden, teiler = ggT Liste

            List<int> primZahlen = new List<int>();
            if (teiler == null)
            {
                primZahlen.Add(2);
                primZahlen.Add(3);
            }

            int i = start;

            if (i % 2 == 0) i++;
            // tests = 0;


            while (i <= end)
            {
                int maxTeiler = (int)Math.Sqrt(i) + 1;
                int j = 0;

                while (true)
                {
                    if (teiler != null && j >= teiler.Count)
                    {
                        primZahlen.Add(i);
                        break;
                    }

                    int n = teiler != null ? teiler[j] : primZahlen[j];
                    int rest = (i % n);
                    // ++tests;

                    if (rest == 0)
                        break; // keine Primzahl
                    if (n >= maxTeiler)
                    {
                        primZahlen.Add(i);
                        break;
                    }

                    ++j;
                }

                i += 2;
            }

            foreach (var primZahl in primZahlen)
            {
                lock (lockObj)
                {
                    liste.Add(primZahl);
                }
            }
        }
    }
}
