using System;
using System.IO;
using System.Text;

namespace GIL
{
    class Program
    {
        const string Version = "0.1";
        public static string DataPath;
        public static string Target = "";

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
            var Files = Directory.GetFiles(Environment.CurrentDirectory, "*.gil");
            if (Files.Length == 0)
            {
                HelperFunctions.WriteError("No GIL project (.gil) in current directory.");
            } else 
            {
                Console.WriteLine("Compiling " + Files[0]);
                new Compiler().Compile(Files[0]);
            }
        }

        static void New(string[] args)
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

        static void Test(string[] args)
        {
            LoadPathway.Load(args[1]);
        }
    }
}
