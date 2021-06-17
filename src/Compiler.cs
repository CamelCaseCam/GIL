using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class Compiler
{
    public void CompileRaw(string path)
    {
        string program = File.ReadAllText(path);
        List<Token> FileTokens = LexerTokens.Lexer.Tokenize(program);
        Project CurrentProject = Parser.Parse(FileTokens);
        
        string FileName = path.Substring(0, path.Length - 3) + "cgil";
        string Compiled = ConvertToCodons(CurrentProject).Replace("\r", "");
        File.WriteAllText(FileName, Compiled);
    }

    string ConvertToCodons(Project CurrentProject)
    {
        CodonEncoding CurrentEncoding = new CodonEncoding(CurrentProject.Target);
        GIL.Program.CurrentEncoding = CurrentEncoding;
        string code = "";

        for (int i = 0; i < CurrentProject.Tokens.Count; i++)
        {
            Token CurrentToken = CurrentProject.Tokens[i];
            switch (CurrentToken.TokenType)
            {
                case LexerTokens.CODON:
                    code += CurrentToken.Value;
                    break;
                case LexerTokens.AMINOCODE:
                    code += CurrentEncoding.GetCode(CurrentToken.Value);
                    break;
                case LexerTokens.AMINOLETTER:
                    code += CurrentEncoding.GetLetter(CurrentToken.Value[0]);
                    break;
                case LexerTokens.AMINOSEQUENCE:
                    foreach (char c in CurrentToken.Value)
                    {
                        if (c == '\n' || c == '\r' || c == ' ' || c == '\t')
                        {
                            continue;
                        }
                        code += CurrentEncoding.GetLetter(c);
                    }
                    break;
                default:
                    break;
            }
        }
        return code;
    }

    public void Compile(string path)
    {
        string program = File.ReadAllText(path).Replace("\r", "").Replace("    ", "\t").Replace("\t", "");
        List<Token> FileTokens = LexerTokens.Lexer.Tokenize(program);
        Project CurrentProject = Parser.Parse(FileTokens);
        string[] Splited = path.Split(new string[] { "/\\" }, StringSplitOptions.None);
        
        string FileName = path.Substring(0, path.Length - 3) + "gb";
        string Compiled = CompileGB(CurrentProject, Splited[Splited.Length - 1]);
        File.WriteAllText(FileName, Compiled);
    }

    string CompileGB(Project CurrentProject, string FileName)
    {
        CodonEncoding CurrentEncoding = new CodonEncoding(CurrentProject.Target);
        GIL.Program.CurrentEncoding = CurrentEncoding;
        string code = "";
        List<Feature> Features = new List<Feature>();
        Stack<Feature> InProgressFeatures = new Stack<Feature>();

        for (int i = 0; i < CurrentProject.Tokens.Count; i++)
        {
            Token CurrentToken = CurrentProject.Tokens[i];
                switch (CurrentToken.TokenType)
                {
                    case LexerTokens.CODON:
                        code += CurrentToken.Value;
                        break;
                    case LexerTokens.AMINOCODE:
                        Console.WriteLine(CurrentToken.Value);
                        break;
                    case LexerTokens.AMINOLETTER:
                        code += CurrentEncoding.GetLetter(CurrentToken.Value[0]);
                        break;
                    case LexerTokens.AMINOSEQUENCE:
                        foreach (char c in CurrentToken.Value)
                        {
                            if (c == '\n' || c == '\r' || c == ' ' || c == '\t')
                            {
                                continue;
                            }
                            code += CurrentEncoding.GetLetter(c);
                        }
                        break;
                    case LexerTokens.BEGINREGION:
                        InProgressFeatures.Push(new Feature(CurrentToken.Value, code.Length + 1, -1));
                        break;
                    case LexerTokens.ENDREGION:
                        if (InProgressFeatures.Count == 0)
                        {
                            HelperFunctions.WriteError("TempError no region to end");
                        }
                        Feature EndedFeature = InProgressFeatures.Pop();
                        EndedFeature.End = code.Length;
                        Features.Add(EndedFeature);
                        break;
                    default:
                        break;
                }
        }
        return new GBSequence() {
            Features = Features.ToArray(),
            Target = CurrentProject.Target,
            FileName = FileName,
            Bases = code
        }.ToString();
    }
}