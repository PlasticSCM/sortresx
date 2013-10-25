using System;
using System.IO;

namespace Codice.SortResX
{
    class Program
    {
        [STAThread]
        static int Main(string[] args)
        {
                ResourceFileSorter fileSorter = null;
                if (args.Length <= 0)
                    return 1;

                if (!CheckArgs(args[0]))
                    return 1;

                try
                {
                    fileSorter = new ResourceFileSorter(args[0]);
                    fileSorter.Sort();
                }
                catch (Exception)
                {
                    Console.WriteLine("================================");
                    Console.WriteLine("Could not sort resources. Abort.");
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
            mFileProcessor = new FileProcessor(path);
        }

        public void Sort()
        {
            if (mFileProcessor != null)
            {
                mFileProcessor.Process();
            }
        }

        private FileProcessor mFileProcessor;
    }
}