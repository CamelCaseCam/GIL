using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public static class Parser
{
    public static Project Parse(List<Token> tokens)
    {
        Project Output = new Project();

        for (int i = 0; i < tokens.Count; i++)
        {
            switch (tokens[i].TokenType)
            {
                case LexerTokens.ENTRYPOINT:    //You can technically use #EntryPoint to start exeution at a specific location. 
                    Output.EntryPoint = i;      //I'll probably remove this later, though
                    break;
                case LexerTokens.SETTARGET:    //sets target organism. In the future, I'll add support for multiple targets
                    Output.Target = tokens[i].Value;
                    break;
                case LexerTokens.SETATTR:
                    string[] AttrAndVal = tokens[i].Value.Split(':');
                    Console.WriteLine(AttrAndVal[1]);
                    switch (AttrAndVal[0])
                    {
                        case "RNAI_Len":
                            if (GIL.Program.StepThrough)
                            {
                                Console.WriteLine($"Setting attribute \"RNAI_Len\" to {AttrAndVal[1]}");
                            }
                            try
                            {
                                GIL.Program.RNAI_Len = int.Parse(AttrAndVal[1]);
                            } catch (Exception e)
                            {
                                if (GIL.Program.StepThrough)
                                {
                                    Console.WriteLine($"Failed to parse {AttrAndVal[1]} to type int with the following exception:\n");
                                    HelperFunctions.WriteError(e.ToString());
                                }
                                HelperFunctions.WriteError($"Error GIL08: Attribute \"{AttrAndVal[0]}\" requires value of type int");
                            }
                            break;
                        default:
                            HelperFunctions.WriteError("Error GIL09: Attribute \"{AttrAndVal}\" does not exist");
                            break;
                    }
                    break;
                case LexerTokens.COMMENT:
                    //Do nothing since it's a comment
                    break;
                case LexerTokens.BEGIN:    //{
                    if (tokens.Count - 1 == i)
                    {
                        HelperFunctions.WriteError("Error GIL05: End token (\"}\") expected");
                    }
                    if (tokens[i + 1].TokenType == LexerTokens.AMINOSEQUENCE)
                    {
                        Output.Tokens.Add(tokens[i + 1]);
                        i += 2;
                        break;
                    }
                    Output.Tokens.Add(tokens[i]);
                    break;
                default:
                    Output.Tokens.Add(tokens[i]);
                    break;
            }
        }
        return Output;
    }

    public static Project Parse(List<Token> tokens, List<string> ReferencedRegions)
    {
        Project Output = new Project();

        for (int i = 0; i < tokens.Count; i++)
        {
            switch (tokens[i].TokenType)
            {
                case LexerTokens.ENTRYPOINT:
                    Output.EntryPoint = i;
                    break;
                case LexerTokens.DEFINESEQUENCE:
                    i = HelperFunctions.GetEnd(tokens, i + 1);
                    break;
                case LexerTokens.DEFOP:
                    i = HelperFunctions.GetEnd(tokens, i + 1);
                    break;
                case LexerTokens.SETTARGET:
                    Output.Target = tokens[i].Value;
                    break;
                case LexerTokens.SETATTR:
                    string[] AttrAndVal = tokens[i].Value.Split(':');
                    switch (AttrAndVal[0])
                    {
                        case "RNAI_Len":
                            if (GIL.Program.StepThrough)
                            {
                                Console.Clear();
                                Console.WriteLine($"Setting attribute \"RNAI_Len\" to {AttrAndVal[1]}");
                                Console.ReadLine();
                            }
                            try
                            {
                                GIL.Program.RNAI_Len = int.Parse(AttrAndVal[1]);
                            } catch (Exception e)
                            {
                                if (GIL.Program.StepThrough)
                                {
                                    Console.WriteLine($"Failed to parse {AttrAndVal[1]} to type int with the following exception:");
                                    HelperFunctions.WriteError(e.ToString());
                                }
                                HelperFunctions.WriteError($"Error GIL08: Attribute \"{AttrAndVal[0]}\" requires value of type int");
                            }
                            break;
                        default:
                            HelperFunctions.WriteError("Error GIL09: Attribute \"{AttrAndVal}\" does not exist");
                            break;
                    }
                    break;
                case LexerTokens.IDENT:
                    Output.Tokens.Add(tokens[i]);
                    break;
                case LexerTokens.COMMENT:
                    //Do nothing since it's a comment
                    break;
                case LexerTokens.BEGIN:
                    if (tokens.Count - 1 == i)
                    {
                        HelperFunctions.WriteError("Error GIL05: End token (\"}\") expected");
                    }
                    if (tokens[i + 1].TokenType == LexerTokens.AMINOSEQUENCE)
                    {
                        Output.Tokens.Add(tokens[i + 1]);
                        i += 2;
                    }
                    Output.Tokens.Add(tokens[i]);    //I forgot that I didn't add this line and spent 2 days trying to 
                    break;                           //figure out why operations weren't working
                case LexerTokens.BEGINREGION:
                    if (ReferencedRegions.Contains(tokens[i].Value))    //If this reference is closed by name, change the token type to REFREGION
                    {
                        Output.Tokens.Add(new Token(LexerTokens.REFREGION, tokens[i].Value));
                    } else
                    {
                        Output.Tokens.Add(tokens[i]);
                    }
                    break;
                case LexerTokens.IMPORT:
                    string LibPath = LibFuncs.GetLibPath(tokens[i].Value);    //find path to file
                    Compiler.ExecutingCompiler = new Compiler();

                    //Get contents of file and tokenize it
                    string program = File.ReadAllText(LibPath).Replace("\r", "").Replace("    ", "\t").Replace("\t", "");
                    List<Token> FileTokens;
                    List<string> NamedTokens;
                    (FileTokens, NamedTokens) = LexerTokens.Lexer.Tokenize(program);
                    Output.ImportedFiles.Add(GIL.Program.EntryFilePath);
                    Output.GetGraphReusableElements(FileTokens, ImportDependencies:true);    //Get all operations and sequences
                    break;
                case LexerTokens.USING:    //Link to specified assembly
                    GetAssembly.Get(tokens[i].Value);
                    break;
                default:
                    Output.Tokens.Add(tokens[i]);
                    break;
            }
        }
        return Output;
    }
}

public class Project
{
    public List<Token> Tokens = new List<Token>();
    public Dictionary<string, RelativeFeature> Sequences = new Dictionary<string, RelativeFeature>();
    public Dictionary<string, RelativeFeature[]> Operations = new Dictionary<string, RelativeFeature[]>();
    public List<string> ImportedFiles = new List<string>();

    public int EntryPoint;
    public string Target;

    public Project Copy()
    {
        return new Project() {Sequences = this.Sequences, Operations = this.Operations, Target = this.Target};
    }

    public override string ToString()
    {
        string Output = $"Targets {Target}\nEntry point: Token {EntryPoint}\nTokens:\n";
        
        foreach (Token t in Tokens)
        {
            Output += t.ToString();
        }

        return Output;
    }

    public void GetReusableElements(List<Token> Tokens)
    {
        GetReusableElements(Tokens.ToArray());
    }

    public void GetReusableElements(Token[] Tokens)    //Get all sequences and operations
    {
        for (int i = 0; i < Tokens.Length; i++)
        {
            List<Token> t;
            string Name;
            switch (Tokens[i].TokenType)
            {
                case LexerTokens.DEFINESEQUENCE:
                    Name = Tokens[i].Value;
                    if (Sequences.ContainsKey(Name))
                    {
                        break;
                    }
                    (i, t) = GetInsideTokens(Tokens, i + 1);
                    AddSequence(t, Name);
                    break;
                default:
                    continue;
            }
        }
        for (int i = 0; i < Tokens.Length; i++)
        {
            List<Token> t;
            string Name;
            switch (Tokens[i].TokenType)
            {
                case LexerTokens.DEFOP:
                    Name = Tokens[i].Value;
                    if (Sequences.ContainsKey(Name))
                    {
                        break;
                    }
                    (i, t) = GetInsideTokens(Tokens, i + 1);
                    AddOp(t, Name);
                    break;
                default:
                    continue;
            }
        }
    }

    public (int, List<Token>) GetInsideTokens(Token[] Tokens, int idx)
    {
        int Layer = 0;
        List<Token> InsideTokens = new List<Token>();
        for (int i = idx; i < Tokens.Length; i++)
        {
            InsideTokens.Add(Tokens[i]);
            switch (Tokens[i].TokenType)
            {
                case LexerTokens.BEGIN:
                    Layer++;
                    break;
                case LexerTokens.END:
                    Layer--;
                    break;
                case LexerTokens.NEWLINE:
                    continue;
                case LexerTokens.COMMENT:
                    continue;
                case LexerTokens.DEFINESEQUENCE:
                    continue;
                case LexerTokens.DEFOP:
                    continue;
                default:
                    break;
            }
            if (Layer <= 0)
            {
                //AddSequence(InsideTokens, name);
                return (i, InsideTokens);
            }
        }

        //AddSequence(InsideTokens, name);
        return (Tokens.Length, InsideTokens);
    }

    void AddSequence(List<Token> tokens, string name)
    {
        if (Sequences.ContainsKey(name))
        {
            return;
        }

        Project InSequence = new Project() {Tokens = tokens, Sequences = this.Sequences, Target = this.Target};
        
        Sequences.Add(name, new RelativeFeature(Compiler.ExecutingCompiler.CompileGB(InSequence, "")));
    }

    void AddOp(List<Token> tokens, string name)
    {
        if (Operations.ContainsKey(name))
        {
            return;
        }

        List<List<Token>> Feats = new List<List<Token>>() {
            new List<Token>()
        };
        int CurrentFeat = 0;
        foreach (Token t in tokens)
        {
            switch (t.TokenType)
            {
                case LexerTokens.INNERCODE:
                    Feats.Add(new List<Token>());
                    CurrentFeat++;
                    break;
                default:
                    Feats[CurrentFeat].Add(t);
                    break;
            }
        }
        RelativeFeature[] RelativeFeatures = new RelativeFeature[Feats.Count];
        for (int i = 0; i < Feats.Count; i++)
        {
            Project InSequence = new Project() {Tokens = Feats[i], Sequences = this.Sequences, 
                Operations = this.Operations, Target = this.Target};
            
            RelativeFeatures[i] = new RelativeFeature(Compiler.ExecutingCompiler.CompileGB(InSequence, ""));
        }
        
        Operations.Add(name, RelativeFeatures);
    }

    public void GetGraphReusableElements(List<Token> tokens, bool ImportDependencies = false)
    {
        GetGraphReusableElements(tokens.ToArray(), ImportDependencies:ImportDependencies);
    }

    public void GetGraphReusableElements(Token[] tokens, bool ImportDependencies = false)
    {
        Dictionary<string, List<Token>> ReusableElements = new Dictionary<string, List<Token>>();

        for (int i = 0; i < tokens.Length; i++)
        {
            switch (tokens[i].TokenType)
            {
                case LexerTokens.DEFOP:
                    goto DefineSequence;
                case LexerTokens.DEFINESEQUENCE:
                    DefineSequence:
                    List<Token> InsideTokens;
                    string SeqName = tokens[i].Value;
                    (i, InsideTokens) = GetInsideTokens(tokens, i);
                    ReusableElements.Add(SeqName, InsideTokens);
                    break;
                case LexerTokens.IMPORT:
                    if (ImportDependencies)
                    {
                        string LibPath = LibFuncs.GetLibPath(tokens[i].Value);    //find path to file
                        if (ImportedFiles.Contains(LibPath))    //Check if file has been imported
                        {
                            if (GIL.Program.StepThrough)    //If we're stepping through, say that we're skipping the file
                            {
                                Console.Clear();
                                Console.WriteLine($"Skipping importing file \"{LibPath}\" because it has already been imported");
                                Console.ReadLine();
                            }
                            break;
                        }
                        ImportedFiles.Add(LibPath);

                        //Get contents of file and tokenize it
                        string program = File.ReadAllText(LibPath).Replace("\r", "").Replace("    ", "\t").Replace("\t", "");
                        List<Token> FileTokens;
                        List<string> NamedTokens;
                        (FileTokens, NamedTokens) = LexerTokens.Lexer.Tokenize(program);
                        GetGraphReusableElements(FileTokens, ImportDependencies:true);    //Get all operations and sequences
                    }
                    break;
                case LexerTokens.USING:    //Link to specified assembly
                    if (ImportDependencies)
                    {
                        string LibPath = LibFuncs.GetLibPath(tokens[i].Value, end:"");    //find path to file
                        if (ImportedFiles.Contains(LibPath))    //Check if file has been imported
                        {
                            if (GIL.Program.StepThrough)    //If we're stepping through, say that we're skipping the file
                            {
                                Console.Clear();
                                Console.WriteLine($"Skipping importing file \"{LibPath}\" because it has already been imported");
                                Console.ReadLine();
                            }
                            break;
                        }
                        ImportedFiles.Add(LibPath);
                        GetAssembly.Get(tokens[i].Value);
                    }
                    break;
                default:
                    break;
            }
        }
        
        foreach (KeyValuePair<string, List<Token>> RE in ReusableElements)
        {
            AddElement(RE.Key, RE.Value, ref ReusableElements);
        }   
    }

    void AddElement(string Name, List<Token> InsideTokens, ref Dictionary<string, List<Token>> ReusableElements)
    {
        foreach (Token t in InsideTokens)
        {
            if (t.TokenType == LexerTokens.CALLOP || 
                (t.TokenType == LexerTokens.IDENT && !LexerTokens.ReservedNames.Contains(t.Value)))
            {
                
                if (!ReusableElements.ContainsKey(t.Value))
                {
                    HelperFunctions.WriteError($"Name \"{t.Value}\" is not defined");
                }
                AddElement(t.Value, ReusableElements[t.Value], ref ReusableElements);
            }
        }
        
        if (InsideTokens[0].TokenType == LexerTokens.DEFINESEQUENCE)
        {
            AddSequence(InsideTokens, Name);
        } else
        {
            AddOp(InsideTokens, Name);
        }
    }
}