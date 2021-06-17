using System;
using System.IO;
using System.Text;

namespace GIL
{
    class Program
    {
        public const string Version = "0.2.3";
        public static string DataPath;    //Path to directory with binaries
        public static string Target = "";
        public static CodonEncoding CurrentEncoding;

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine($"GIL compiler version {Version}");
                return;
            }
            DataPath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

            switch (args[0])
            {
                case "compile":
                    Run(args);
                    break;
                case "new":
                    New(args);
                    break;
                case "test":
                    Test(args);
                    break;
                case "build-pathway":
                    PathwayBuilder.BuildPathway(Environment.CurrentDirectory + "\\" + args[1]);
                    break;
                default:
                    break;
            }
            
        }

        static void Run(string[] args)
        {
            if (args.Length > 1)
            {
                if (args[1].Contains(':'))
                {
                    Console.WriteLine("Compiling GIL file at " + args[1]);
                    new Compiler().Compile(args[1]);
                } else
                {
                    string Path = Environment.CurrentDirectory + "\\" + args[1];
                    Console.WriteLine("Compiling GIL file at " + Path);
                    new Compiler().Compile(Path);
                }
                return;
            }
            var Files = Directory.GetFiles(Environment.CurrentDirectory, "*.gil");    //compile first .gil file in directory
            if (Files.Length == 0)
            {
                HelperFunctions.WriteError("No GIL project (.gil) in current directory.");
            } else 
            {
                Console.WriteLine("Compiling " + Files[0]);
                new Compiler().Compile(Files[0]);
            }
        }

        static void New(string[] args)    //wip
        {
            if (args.Length < 2)
            {
                Console.WriteLine("You must specify which template to load");
                return;
            }

            if (File.Exists(DataPath + $"/Templates/{args[1]}.gil"))
            {
                Console.WriteLine($"Loading template \"{args[1]}\"");
                string Template = File.ReadAllText(DataPath + $"/Templates/{args[1]}.gil");
                string ProjectPath = Environment.CurrentDirectory + "\\" + 
                    Path.GetFileName(Environment.CurrentDirectory) + ".gil";
                File.WriteAllText(ProjectPath, Template);
                return;
            }
            HelperFunctions.WriteError($"Template {args[1]} not found");
        }
        
        static GBSequence GBS = new GBSequence() {
            Bases = "ATGTTTTAG",
            Target = "Saccharomyces cerevisiae",
            FileName = "Test.gil",
            Features = new Feature[] {
                new Feature("Start codon", 1, 3),
                new Feature("Stop codon", 7, 9)
            }
        };
        static void Test(string[] args)
        {
            Console.Write(GBS);
        }
    }
}
