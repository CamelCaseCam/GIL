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
                case LexerTokens.COMMENT:
                    //Do nothing since it's a comment
                    break;
                case LexerTokens.BEGIN:    //{
                    if (tokens.Count - 1 == i)
                    {
                        HelperFunctions.WriteError("ERROR: End token ('}') expected");
                    }
                    if (tokens[i + 1].TokenType == LexerTokens.AMINOSEQUENCE)
                    {
                        Output.Tokens.Add(tokens[i + 1]);
                        i++;
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
                case LexerTokens.IDENT:
                    Output.Tokens.Add(tokens[i]);
                    break;
                case LexerTokens.COMMENT:
                    //Do nothing since it's a comment
                    break;
                case LexerTokens.BEGIN:
                    if (tokens.Count - 1 == i)
                    {
                        HelperFunctions.WriteError("ERROR: End token ('}') expected");
                    }
                    if (tokens[i + 1].TokenType == LexerTokens.AMINOSEQUENCE)
                    {
                        Output.Tokens.Add(tokens[i + 1]);
                        i++;
                    }
                    Output.Tokens.Add(tokens[i]);    //I forgot that I didn't add this line and spent 2 days trying to 
                    break;                           //figure out why operations weren't working
                case LexerTokens.BEGINREGION:
                    if (ReferencedRegions.Contains(tokens[i].Value))
                    {
                        Output.Tokens.Add(new Token(LexerTokens.REFREGION, tokens[i].Value));
                    } else
                    {
                        Output.Tokens.Add(tokens[i]);
                    }
                    break;
                case LexerTokens.IMPORT:
                    string LibPath = LibFuncs.GetLibPath(tokens[i].Value);
                    Compiler.ExecutingCompiler = new Compiler();

                    string program = File.ReadAllText(LibPath).Replace("\r", "").Replace("    ", "\t").Replace("\t", "");
                    List<Token> FileTokens;
                    List<string> NamedTokens;
                    (FileTokens, NamedTokens) = LexerTokens.Lexer.Tokenize(program);
                    Output.GetReusableElements(FileTokens);
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

    public void GetReusableElements(Token[] Tokens)
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
        Project InSequence = new Project() {Tokens = tokens, Sequences = this.Sequences, Target = this.Target};
        
        Sequences.Add(name, new RelativeFeature(Compiler.ExecutingCompiler.CompileGB(InSequence, "")));
    }

    void AddOp(List<Token> tokens, string name)
    {
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
            Project InSequence = new Project() {Tokens = Feats[i], Sequences = this.Sequences, Target = this.Target};
            
            RelativeFeatures[i] = new RelativeFeature(Compiler.ExecutingCompiler.CompileGB(InSequence, ""));
        }
        Operations.Add(name, RelativeFeatures);
    }
}