using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace GIL
{
    class Program
    {
        public const string Version = "0.3.1";
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
            DataPath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);    //Get path to binaries

            switch (args[0])
            {
                case "compile":
                    Run(args);
                    break;
                case "new":    //create new GIL file
                    New(args);
                    break;
                case "debug":
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
                if (args[1].Contains(':'))    //If it's a full path to file
                {
                    Console.WriteLine("Compiling GIL file at " + args[1]);
                    new Compiler().Compile(args[1]);
                } else    //relative path
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
                HelperFunctions.WriteError("Error GIL03: No GIL project (.gil) in current directory");
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

            if (File.Exists(DataPath + $"/Templates/{args[1]}.gil"))    //Copy file with same name from templates folder
            {
                Console.WriteLine($"Loading template \"{args[1]}\"");
                string Template = File.ReadAllText(DataPath + $"/Templates/{args[1]}.gil");
                string ProjectPath = Environment.CurrentDirectory + "\\" + 
                    Path.GetFileName(Environment.CurrentDirectory) + ".gil";
                File.WriteAllText(ProjectPath, Template);
                return;
            }
            HelperFunctions.WriteError($"Error GIL04: Project template \"{args[1]}\" not found");
        }
        
        static void Test(string[] args)    //just for testing
        {
            Console.Clear();
            Console.Write(DebugMenu);

            switch (Console.ReadLine())
            {
                case "1":
                    Tokenize(args);
                    break;
                case "2":
                    Parse(args);
                    break;
                case "3":
                    TestFeature(args);
                    break;
                default:
                    break;
            }
        }

        public const string DebugMenu = @"
        GIL Debug Menu        

Options: 
[1]Tokenize - Tokenizes GIL file and outputs tokens to console
[2]Parse - Tokenizes and parses GIL file and outputs tokens to console
[3]Test feature - Executes TestFeature method in Program.cs, used for testing unfinished features

";
    
        static void TestFeature(string[] args)
        {

        }

        static void Tokenize(string[] args)
        {
            Console.Clear();
            string FilePath = "";
            if (args.Length > 1)
            {
                if (args[1].Contains(':'))    //If it's a full path to file
                {
                    Console.WriteLine("Compiling GIL file at " + args[1]);
                    FilePath = args[1];
                } else    //relative path
                {
                    FilePath = Environment.CurrentDirectory + "\\" + args[1];
                    Console.WriteLine("Compiling GIL file at " + FilePath);
                }
            } else
            {
                var Files = Directory.GetFiles(Environment.CurrentDirectory, "*.gil");    //compile first .gil file in directory
                if (Files.Length == 0)
                {
                    HelperFunctions.WriteError("Error GIL03: No GIL project (.gil) in current directory");
                } else 
                {
                    Console.WriteLine("Compiling " + Files[0]);
                    FilePath = Files[0];
                }
            }

            string program = File.ReadAllText(FilePath).Replace("\r", "").Replace("    ", "\t").Replace("\t", "");
            List<Token> FileTokens;
            List<string> NamedTokens;
            (FileTokens, NamedTokens) = LexerTokens.Lexer.Tokenize(program);
            
            foreach (Token t in FileTokens)
            {
                Console.WriteLine(t);
            }
        }

        static void Parse(string[] args)
        {
            Console.Clear();
            string FilePath = "";
            if (args.Length > 1)
            {
                if (args[1].Contains(':'))    //If it's a full path to file
                {
                    Console.WriteLine("Compiling GIL file at " + args[1]);
                    FilePath = args[1];
                } else    //relative path
                {
                    FilePath = Environment.CurrentDirectory + "\\" + args[1];
                    Console.WriteLine("Compiling GIL file at " + FilePath);
                }
            } else
            {
                var Files = Directory.GetFiles(Environment.CurrentDirectory, "*.gil");    //compile first .gil file in directory
                if (Files.Length == 0)
                {
                    HelperFunctions.WriteError("Error GIL03: No GIL project (.gil) in current directory");
                } else 
                {
                    Console.WriteLine("Compiling " + Files[0]);
                    FilePath = Files[0];
                }
            }

            string program = File.ReadAllText(FilePath).Replace("\r", "").Replace("    ", "\t").Replace("\t", "");
            List<Token> FileTokens;
            List<string> NamedTokens;
            (FileTokens, NamedTokens) = LexerTokens.Lexer.Tokenize(program);
            
            Project CurrentProject = Parser.Parse(FileTokens, NamedTokens);

            foreach (Token t in CurrentProject.Tokens)
            {
                Console.WriteLine(t);
            }
        }

    }
}
