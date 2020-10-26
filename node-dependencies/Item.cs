using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace node_dependencies
{
    public class Item
    {
        public List<string> DependenciesNames { get; set; }  = new List<string>();
        public List<Item> Dependencies { get; set; } = new List<Item>();
        public List<Item> DependsOn { get; set; } = new List<Item>();
        public string Name { get; set; }
        public string Directory { get; set; }

        public Item(string directory)
        {
            this.Directory = directory.Replace(Finder.basePath + Path.DirectorySeparatorChar, string.Empty);
            DependenciesNames = new List<string>();
            Dependencies = new List<Item>();
            DependsOn = new List<Item>();
        }

        public void PrintForward(int i = 0)
        {
            Console.WriteLine($"{Directory} [{DependsOn.Count}][{Dependencies.Count}]");
            foreach (var item in DependsOn)
            {
                // if (item == this) continue;
                for (int j = 0; j < i; j++)
                {
                    Console.Write("|\t");
                }
                Console.Write("├───");
                item.PrintForward(i + 1);
            }
        }

        public void PrintBackward(int i = 0)
        {
            Console.WriteLine($"{Directory} [{DependsOn.Count}][{Dependencies.Count}]");
            foreach (var item in Dependencies)
            {
                for (int j = 0; j < i; j++)
                {
                    Console.Write("|\t");
                }
                Console.Write("├───");
                item.PrintBackward(i + 1);
            }
        }

        public void FindDependencies(Finder finder)
        {
            int imports = 0;

            var lines = File.ReadAllLines(Finder.basePath +Path.AltDirectorySeparatorChar + Directory);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("import"))
                {
                    if (lines[i].Contains("from"))
                    {
                        var v = lines[i].Split("from");
                        if (v[v.Length - 1].Contains("."))
                        {
                            var it = v[v.Length - 1].Replace("\"", string.Empty).Replace("'", string.Empty).Replace(";", string.Empty);
                            test(finder, it);
                        }
                    }
                }
                else if (lines[i].Contains("require"))
                {
                    var v = lines[i].Split("require(");
                    var it = v[v.Length - 1].Replace("'", string.Empty).Replace(");", string.Empty);
                    test(finder, it);
                }
            }
        }

        void test(Finder finder, string it)
        {
            if (it[0] == ' ')
            {
                it = it.Remove(0, 1);
            }
            if (it == "..") return;
            var sp = it.Split('/');
            var currentPath = Directory.Replace(Name, string.Empty);
            for (int j = 0; j < sp.Length; j++)
            {
                if (sp[j] == ".")
                {

                }
                else if (sp[j] == "..")
                {
                    var vv = currentPath.Split(Path.DirectorySeparatorChar);
                    currentPath = currentPath.Replace(Path.DirectorySeparatorChar + vv[vv.Length - 2], string.Empty);
                }
                else
                {
                    currentPath += sp[j];
                    if (j != sp.Length - 1)
                    {
                        currentPath += Path.DirectorySeparatorChar;
                    }
                }
            }

            Item found = finder.Items.Where(e => e.Directory.Contains(currentPath)).FirstOrDefault();
            if (found != null)
            {
                if (found != this)
                {
                    found.Dependencies.Add(this);
                    DependsOn.Add(found);
                }
            }
        }
    }
}
