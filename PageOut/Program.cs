using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace PageOut
{
    class Program
    {
        static void Main(string[] args)
        {
            var qtItens = 200000;

            //PerformanceLoopParallel(qtItens);

            PerformanceLoop(qtItens);

            Console.ReadLine();

        }



        static void PerformanceLoop(int qtItems)
        {
            var stopwatch = new Stopwatch();

            var lst = GetData(qtItems);

            stopwatch.Start();

            Print(lst);

            var ts = stopwatch.ElapsedMilliseconds;
            Console.WriteLine(ts);
        }

        static void PerformanceLoopParallel(int qtItems)
        {
            var stopwatch = new Stopwatch();
            var qtPerPage = 10000;
            var pages = qtItems / qtPerPage;

            var lst = GetData(qtItems);

            stopwatch.Start();

            Enumerable.Range(0, pages)
                .Select(x => lst.Skip(x * qtPerPage).Take(qtPerPage))
                .AsParallel()
                .WithDegreeOfParallelism(Environment.ProcessorCount)
                .ForAll(x =>
                {
                    //Console.WriteLine($"Thread: {Thread.CurrentThread.ManagedThreadId}");
                    Print(x);
                });

            var ts = stopwatch.ElapsedMilliseconds;
            Console.WriteLine(ts);
        }

        static void Print(IEnumerable<string> data)
        {
            Parallel.ForEach(data, x =>
            {
                Console.WriteLine($"Value: {x}");
            });


            //foreach (var item in data)
            //{
            //    //Console.WriteLine($"=========== Thread: {Thread.CurrentThread.ManagedThreadId}");
            //    Console.WriteLine($"Value: {item}");
            //}

        }

        static List<string> GetData(int qtItems)
        {
            var lst = new List<string>();

            for (int i = 0; i < qtItems; i++)
            {
                lst.Add(i.ToString());
            }

            return lst;
        }
    }
}
