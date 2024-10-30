using RectiPack;
using System.Diagnostics;


namespace Demo {

    static internal class Program {

        const int MAX_BITMAP_SIZE = 23170;
        static RectiPack.Rect[] rects = [new Rect() { width = 128, height = 32 }, new Rect() { width = 128, height = 64 }, new Rect() { width = 64, height = 32 }, new Rect() { width = 64, height = 32 }];

        static void Main(string[] args) {
            Console.WriteLine("Welcome to the RectiPack demo");
            
            // Randomly generate a bunch of rects
            List<Rect> randomRects = new List<Rect>();

            for (int i = 0; i < 10000; i++) {
                randomRects.Add(new RectiPack.Rect() { width = (uint)Random.Shared.Next(20, 200), height = (uint)Random.Shared.Next(20, 200) });
            }


            Console.WriteLine("Packing " + randomRects.Count + " rects");

            Stopwatch sw = Stopwatch.StartNew();

            //var result = RectiPack.RectiPack.Pack(rects);
            //var result = RectiPack.RectiPack.Pack(randomRects.ToArray());
            var result = RectiPack.RectiPack.Pack(randomRects.ToArray(), MAX_BITMAP_SIZE, -1, RectiPack.RectiPack.DEFAULT_SORTING_ORDER, Packing.Dimensions.Both);

            sw.Stop();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Packed in {((double)sw.ElapsedTicks / (double)Stopwatch.Frequency) * 1000} ms");
            Console.ForegroundColor = ConsoleColor.White;

            if (result == null) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Failed to pack rects");
                Console.ForegroundColor = ConsoleColor.White;

                return;
            }

            Package p = result ?? throw new Exception("Invalid result");

            // Ensure that no rectangle overlaps
            // This seems to work, so I'll just leave this here in case something breaks
            /*for (int i = 0; i < p.placements.Length; i++) {
                for (int i2 = i + 1; i2 < p.placements.Length; i2++) {
                    if (p.placements[i].x + p.placements[i].width > p.placements[i2].x &&
                        p.placements[i].x < p.placements[i2].x + p.placements[i2].width &&
                        p.placements[i].y + p.placements[i].height > p.placements[i2].y &&
                        p.placements[i].y < p.placements[i2].y + p.placements[i2].height) { throw new Exception("Found two overlapping rectangles"); }
                }
            }*/


            Console.WriteLine($"Result: {p.boundingRect}({p.usedOrder})");
            Console.WriteLine($"Efficiency: {Math.Round(randomRects.Sum(x => x.Area()) / (double)p.boundingRect.Area() * 100d, 2)}%");
            Console.WriteLine();

            Console.Write("Render image (y/n): ");
            //while (!Console.KeyAvailable) { Thread.Sleep(50); }
            //if (Console.ReadKey().Key != ConsoleKey.Y) { return; }
            if (Console.ReadLine().ToLower() != "y") { return; }
            Console.WriteLine();


            Visual.RenderImage(p, "demo.png");



            Console.WriteLine("Done");
        }


    }
}
