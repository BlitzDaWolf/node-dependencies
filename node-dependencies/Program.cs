using System;
using System.IO;

namespace node_dependencies
{
    class Program
    {
        public static readonly string BasePath = @"D:\dev\express";

        static void Main(string[] args)
        {
            Finder f = new Finder();
            f.Search(BasePath);

            f.PrintForward(false);
        }
    }
}
