using System.Diagnostics;

namespace SimpleThreadApp
{
    class SimpleThreadApp
    {
        static int counter = 0;
        static object lockObj = new object();

        public static void ThreadWorker(int id, int iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                if (counter % id == 0)
                {
                    Console.WriteLine("ID: {0,3} Counter: {1,8} Modulo: {2}", id, counter, counter % id);
                }

                lock (lockObj)
                {
                    counter++;
                }
            }
        }

        public static void OldMain(string[] args)
        {
            int threadCounter = int.Parse(args[0]);
            int iterCounter = int.Parse(args[1]);

            List<Thread> threads = new List<Thread>();

            for (int i = 0; i < threadCounter; i++)
            {
                int id = i + 2;
                Thread t = new Thread(() => ThreadWorker(id, iterCounter));
                threads.Add(t);

                t.Start();
            }

            foreach (Thread t in threads)
            {
                t.Join();
            }
        }

    }
}