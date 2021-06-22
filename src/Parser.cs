using System;
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
                case LexerTokens.ENTRYPOINT:
                    Output.EntryPoint = i;
                    break;
                case LexerTokens.SETTARGET:
                    Output.Target = tokens[i].Value;
                    break;
                case LexerTokens.IDENT:
                    //To be implemented
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
                    break;
                case LexerTokens.END:
                    //To be implemented
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
                    break;
                case LexerTokens.END:
                    //To be implemented
                    break;
                case LexerTokens.BEGINREGION:
                    if (ReferencedRegions.Contains(tokens[i].Value))
                    {
                        Output.Tokens.Add(new Token(LexerTokens.REFREGION, tokens[i].Value));
                    } else
                    {
                        Output.Tokens.Add(tokens[i]);
                    }
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

    public int EntryPoint;
    public string Target;

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
            switch (Tokens[i].TokenType)
            {
                case LexerTokens.DEFINESEQUENCE:
                    i = GetSequence(Tokens, i + 1, Tokens[i].Value);
                    break;
                default:
                    continue;
            }
        }
    }

    int GetSequence(Token[] Tokens, int idx, string name)
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
                default:
                    break;
            }
            if (Layer <= 0)
            {
                AddSequence(InsideTokens, name);
                return i;
            }
        }

        AddSequence(InsideTokens, name);
        return Tokens.Length;
    }

    void AddSequence(List<Token> tokens, string name)
    {
        Project InSequence = new Project() {Tokens = tokens, Sequences = this.Sequences, Target = this.Target};
        
        Sequences.Add(name, new RelativeFeature(Compiler.ExecutingCompiler.CompileGB(InSequence, "")));
    }
}