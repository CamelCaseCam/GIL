using System;
using System.Collections.Generic;

public static class HelperFunctions
{
    public static void WriteWarning(string text)
    {
        var OriginalColour = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(text);
        Console.ForegroundColor = OriginalColour;
    }

    public static void WriteError(string text)
    {
        var OriginalColour = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(text);
        Console.ForegroundColor = OriginalColour;
        Environment.Exit(0);
    }

    public static int GetEnd(List<Token> Tokens, int i)
    {
        return GetEnd(Tokens.ToArray(), i);
    }

    public static int GetEnd(Token[] Tokens, int idx)
    {
        int Depth = 0;
        for (int i = idx; i < Tokens.Length; i++)
        {
            switch (Tokens[i].TokenType)
            {
                case LexerTokens.BEGIN:
                    Depth++;
                    break;
                case LexerTokens.END:
                    Depth--;
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
            if (Depth <= 0)
            {
                return i;
            }
        }
        return Tokens.Length;
    }
}