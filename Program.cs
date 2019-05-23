using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace GameOfLife {
    public class Map {
        private readonly int width;
        private readonly int height;
        private readonly bool[] map;

        public Map(int width, int height) {
            this.width = width;
            this.height = height;
            this.map = new bool[width * height];
        }

        public void Set(int x, int y, bool state) { if (x >= 0 && y >= 0 && x < this.width && y < this.height) this.map[y * this.width + x] = state; }
        public bool Get(int x, int y) => x >= 0 && y >= 0 && x < this.width && y < this.height ? this.map[y * this.width + x] : false;
        public void Clear() => Array.Clear(this.map, 0, this.map.Length);
    }

    public class Simulator {
        private readonly int width;
        private readonly int height;
        private Map current;
        private Map next;

        public Simulator(int width, int height, List<(int x, int y)> initialAlive) {
            this.width = width;
            this.height = height;
            this.current = new Map(width, height);
            this.next = new Map(width, height);

            foreach (var (x, y) in initialAlive)
                this.current.Set(x, y, true);
        }

        private int GetNeighborCount(int x, int y) {
            var cnt = 0;

            if (this.current.Get(x - 1, y)) cnt++;
            if (this.current.Get(x + 1, y)) cnt++;
            if (this.current.Get(x, y - 1)) cnt++;
            if (this.current.Get(x, y + 1)) cnt++;

            if (this.current.Get(x - 1, y - 1)) cnt++;
            if (this.current.Get(x - 1, y + 1)) cnt++;
            if (this.current.Get(x + 1, y - 1)) cnt++;
            if (this.current.Get(x + 1, y + 1)) cnt++;

            return cnt;
        }

        public void Step() {
            for (var y = 0; y < this.height; y++) {
                for (var x = 0; x < this.width; x++) {
                    var count = this.GetNeighborCount(x, y);
                    var state = false;

                    if (this.current.Get(x, y)) {
                        if (count < 2) state = false;
                        else if (count > 3) state = false;
                        else state = true;
                    }
                    else {
                        if (count == 3) state = true;
                        else state = false;
                    }

                    this.next.Set(x, y, state);

                }
            }

            this.current.Clear();

            var tmp = this.current;
            this.current = this.next;
            this.next = tmp;
        }

        public List<(int x, int y)> GetLiveCells() {
            var lst = new List<(int, int)>();

            for (var y = 0; y < this.height; y++)
                for (var x = 0; x < this.width; x++)
                    if (this.current.Get(x, y))
                        lst.Add((x, y));

            return lst;
        }
    }

    public class Program {
        static void Main(string[] args) {
            var steps = int.Parse(args[0]);
            var width = int.Parse(args[1]);
            var height = int.Parse(args[2]);
            var cells = new List<(int, int)>();

            while (Console.ReadLine() is string str) {
                var parts = str.Split(' ');

                cells.Add((int.Parse(parts[0]), int.Parse(parts[1])));
            }

            var sim = new Simulator(width, height, cells);

            render(sim.GetLiveCells());

            var start = DateTime.UtcNow;
            for (var i = 0; i < steps; i++) {
                sim.Step();

                //render(sim.GetLiveCells());
                //Thread.Sleep(100);
            }
            var end = DateTime.UtcNow;

            var final = sim.GetLiveCells();

            render(final);

            final.OrderBy(f => f.y).ThenBy(f => f.x).ToList();

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine($"Took {(end - start).TotalMilliseconds:N3} ms");
            Console.WriteLine(string.Join(Environment.NewLine, final.Select(f => $"{f.x} {f.y}")));

            void render(List<(int x, int y)> lst) {
                Console.Clear();

                foreach (var (x, y) in lst) {
                    Console.SetCursorPosition(x, y);
                    Console.Write((char)0x2588);
                }
            }
        }
    }
}
