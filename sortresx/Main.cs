using System;
using System.IO;

namespace Codice.SortResX
{
    class Program
    {
        [STAThread]
        static int Main(string[] args)
        {
            if (args.Length <= 0)
            {
                PrintUsage("<none>");
                return 1;
            }

            var filepath = args[0];
            if (!CheckArgs(filepath))
            {
                Console.WriteLine("The file specified '{0}' doesn't exist!", filepath);
                return 1;
            }

            try
            {
                new ResourceFileSorter(filepath).Sort();
            }
            catch (Exception)
            {
                Console.WriteLine("================================");
                Console.WriteLine("Could not sort resources. Abort.");
                return 2;
            }

			return 0;
        }

        static void PrintUsage(string argument)
        {
            Console.WriteLine("Invalid argument:" + argument + "\nUsage: sortresx file_to_sort");
        }

        static bool CheckArgs(string filepath)
        {
            return File.Exists(filepath);
        }
    }

    internal class ResourceFileSorter
    {
        public ResourceFileSorter(string path)
        {
            _mFileProcessor = new FileProcessor(path);
        }

        public void Sort()
        {
            _mFileProcessor?.Process();
        }

        private readonly FileProcessor _mFileProcessor;
    }
}