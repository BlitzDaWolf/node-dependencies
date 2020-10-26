using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace node_dependencies
{
    public class Finder
    {
        public static string basePath { get; private set; } = "";

        private List<Item> items = new List<Item>();
        public List<Item> Items { get => items; }

        public Finder()
        {
            items = new List<Item>();
        }

        public void Search(string path)
        {
            basePath = path;
            
            string[] exc = { ".js", ".jsx", ".ts", ".tsx" };
            string[] exculde = { "test", "examples" };

            Queue<string> searchDirectories = new Queue<string>();
            searchDirectories.Enqueue(path);

            int directories = 0;

            while (searchDirectories.Count > 0)
            {
                string current = searchDirectories.Dequeue();

                var tempfiles = Directory.GetFiles(current).ToList();

                List<string> files = new List<string>();
                exc.ToList().ForEach(e =>
                {
                    files.AddRange(
                        tempfiles.Where(d => 
                            d.EndsWith(e) &&
                            !(d.Contains("examples") || d.Contains("test")))
                        );
                });

                files.ForEach(e =>
                {
                    var v = e.Split(Path.DirectorySeparatorChar);
                    string fullItemName = v[v.Length - 1];
                    Item i = new Item(e) { Name = fullItemName };
                    items.Add(i);
                });

                var folders = Directory.GetDirectories(current).ToList().Where(e => !(e.Contains(Path.DirectorySeparatorChar + ".") || e.Contains("node_"))).ToList();
                folders.ForEach(e => searchDirectories.Enqueue(e));
                directories++;
            }

            Console.WriteLine($"Found: {this.items.Count} files. {directories} directories searched");

            items.ForEach(e =>
            {
                e.FindDependencies(this);
            });
        }

        public void PrintForward(bool showzero = false)
        {
            var show = (showzero) ? -1 : 0;

            var r = items.OrderBy(e => e.Dependencies.Count).Where(e => e.DependsOn.Count != show);
            r.ToList().ForEach(e => e.PrintForward());
        }

        public void PrintBackward(bool showzero = false)
        {
            var show = (showzero) ? -1 : 0;
            var r = items.OrderBy(e => -e.Dependencies.Count).Where(e => e.DependsOn.Count == 0 && e.Dependencies.Count > show);
            r.ToList().ForEach(e => e.PrintBackward());
        }
    }
}
