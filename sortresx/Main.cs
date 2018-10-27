using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Codice.SortResX
{
    class Program
    {
        enum ReturnCodes
        {
            [Description("Success with file sorted")]
            SuccessFileSorted = 0,
            [Description("Success but the file didn't need to be sorted")]
            SuccessButNothingToSort = 1,
            [Description("Help displayed")]
            Help = 2,
            [Description("Bad arguments passed to the tool")]
            BadArguments = 3,
            [Description("Error during processing")]
            ErrorDuringProcessing = 4,
        }

        [STAThread]
        static int Main(string[] args)
        {
            if (args.Length <= 0)
            {
                PrintUsage("<none>");
                return (int) ReturnCodes.Help;
            }

            var filepath = args[0];
            if (!CheckArgs(filepath))
            {
                Console.WriteLine("The file specified '{0}' doesn't exist!", filepath);
                return (int) ReturnCodes.BadArguments;
            }

            try
            {
                var doneSomethingSuccessfully = new ResourceFileSorter(filepath).Sort();
                return (int) (doneSomethingSuccessfully ? ReturnCodes.SuccessFileSorted : ReturnCodes.SuccessButNothingToSort);
            }
            catch (Exception)
            {
                Console.WriteLine("================================");
                Console.WriteLine("Could not sort resources. Abort.");
                return (int)ReturnCodes.ErrorDuringProcessing;
            }
        }

        static void PrintUsage(string argument)
        {
            Console.WriteLine("Invalid argument:" + argument + "\nUsage: sortresx file_to_sort");
            Console.WriteLine("Return codes:\n");

            var myEnumDescriptions = from ReturnCodes n in Enum.GetValues(typeof(ReturnCodes))
                select new {ID = (int) n, Name = GetEnumDescription(n)};
            foreach (var returnCode in myEnumDescriptions)
            {
                Console.WriteLine("     " + returnCode.Name + ":" + returnCode.ID + "\n");
            }
        }

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                    typeof(DescriptionAttribute),
                    false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
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

        public bool Sort()
        {
            return _mFileProcessor?.Process() ?? false;
        }

        private readonly FileProcessor _mFileProcessor;
    }
}