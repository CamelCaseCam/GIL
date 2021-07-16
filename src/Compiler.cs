using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class Compiler
{
    public static Compiler ExecutingCompiler = null;
    public void CompileRaw(string path)
    {
        ExecutingCompiler = this;
        string program = File.ReadAllText(path);
        List<Token> FileTokens = LexerTokens.Lexer.OldTokenize(program);
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
        ExecutingCompiler = this;
        string program = File.ReadAllText(path).Replace("\r", "").Replace("    ", "\t").Replace("\t", "");
        List<Token> FileTokens;
        List<string> NamedTokens;
        (FileTokens, NamedTokens) = LexerTokens.Lexer.Tokenize(program);

        Project CurrentProject = Parser.Parse(FileTokens, NamedTokens);
        CurrentProject.GetGraphReusableElements(FileTokens);
        string[] Splited = path.Split(new string[] { "/\\" }, StringSplitOptions.None);
        
        string FileName = path.Substring(0, path.Length - 3) + "gb";
        string Compiled = CompileGB(CurrentProject, Splited[Splited.Length - 1]).ToString();
        File.WriteAllText(FileName, Compiled);
    }

    public GBSequence CompileGB(Project CurrentProject, string FileName)
    {
        CodonEncoding CurrentEncoding = new CodonEncoding(CurrentProject.Target);
        GIL.Program.CurrentEncoding = CurrentEncoding;
        string code = "";
        List<Feature> Features = new List<Feature>();
        Stack<Feature> InProgressFeatures = new Stack<Feature>();
        Dictionary<string, Feature> ReferencedRegions = new Dictionary<string, Feature>();

        for (int i = 0; i < CurrentProject.Tokens.Count; i++)
        {
            Token CurrentToken = CurrentProject.Tokens[i];
            switch (CurrentToken.TokenType)
            {
                case LexerTokens.CODON:
                    code += CurrentToken.Value;
                    break;
                case LexerTokens.AMINOCODE:
                    break;
                case LexerTokens.AMINOLETTER:
                    code += CurrentEncoding.GetLetter(CurrentToken.Value[0]);
                    break;
                case LexerTokens.IDENT:
                    if (LexerTokens.ReservedNames.Contains(CurrentToken.Value))    //Check if token is a reserved name
                    {
                        continue;
                    }

                    if (!CurrentProject.Sequences.ContainsKey(CurrentToken.Value))    //Check if name has been defined
                    {
                        HelperFunctions.WriteError($"The name \"{CurrentToken.Value}\" does not exist");
                    }
                    Feature SequenceFeature = new Feature(CurrentToken.Value, code.Length, -1);
                    (Feature[] features, string dna) = CurrentProject.Sequences[CurrentToken.Value].Get(i);
                    
                    foreach (Feature f in features)
                    {
                        Features.Add(f);
                    }
                    code += dna;
                    SequenceFeature.End = code.Length;
                    Features.Add(SequenceFeature);
                    break;
                case LexerTokens.CALLOP:
                    if (LexerTokens.ReservedNames.Contains(CurrentToken.Value))    //Check if token is a reserved name
                    {
                        continue;
                    }
                    
                    List<Token> InsideTokens;
                    Feature OpFeature;
                    if (!CurrentProject.Operations.ContainsKey(CurrentToken.Value))    //Check if name has been defined
                    {
                        if (GetOp.Operations.Contains(CurrentToken.Value))
                        {
                            (i, InsideTokens) = CurrentProject.GetInsideTokens(CurrentProject.Tokens.ToArray(), i + 1);
                            RelativeFeature RF = GetOp.Call(CurrentToken.Value, InsideTokens);

                            if (RF == null)
                            {
                                continue;
                            }

                            (features, dna) = RF.Get(code.Length);
                            OpFeature = new Feature(CurrentToken.Value, code.Length, -1);
                            goto OpCalled;
                        }
                        HelperFunctions.WriteError($"The name \"{CurrentToken.Value}\" does not exist");
                    }
                    OpFeature = new Feature(CurrentToken.Value, code.Length, -1);

                    (i, InsideTokens) = CurrentProject.GetInsideTokens(CurrentProject.Tokens.ToArray(), i + 1);
                    Project InOp = CurrentProject.Copy();
                    InOp.Tokens = InsideTokens;
                    RelativeFeature InnerCode = new RelativeFeature(CompileGB(InOp, ""));

                    (features, dna) = CallOp.Call(CurrentProject.Operations[CurrentToken.Value], 
                        InnerCode, code.Length);
                    
                    OpCalled:
                    
                    foreach (Feature f in features)
                    {
                        Features.Add(f);
                    }
                    code += dna;
                    OpFeature.End = code.Length;
                    Features.Add(OpFeature);
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
                case LexerTokens.REFREGION:
                    ReferencedRegions.Add(CurrentToken.Value, new Feature(CurrentToken.Value, code.Length + 1, -1));
                    break;
                case LexerTokens.ENDREGION:
                    if (InProgressFeatures.Count == 0 && ReferencedRegions.Count == 0)
                    {
                        HelperFunctions.WriteError("Error GIL01: No region to end");
                    }
                    
                    Feature EndedFeature;
                    if (CurrentToken.Value != "" && CurrentToken.Value != " ")
                    {
                        if (!ReferencedRegions.ContainsKey(CurrentToken.Value))
                        {
                            HelperFunctions.WriteError($"Error GIL02: Region \"{CurrentToken.Value}\" does not exist");
                        }
                        EndedFeature = ReferencedRegions[CurrentToken.Value];
                        ReferencedRegions.Remove(CurrentToken.Value);
                    } else
                    {
                        EndedFeature = InProgressFeatures.Pop();
                    }

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
        };
    }
}