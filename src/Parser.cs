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
}

public class Project
{
    public List<Token> Tokens = new List<Token>();

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
}