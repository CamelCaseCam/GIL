using System;
using System.Text;
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

    public static string[] GetArgs(string line)
    {
        List<string> Output = new List<string>();
        bool InQuotes = false;

        string CurrentString = "";
        for (int i = 0; i < line.Length; i++)
        {
            switch (line[i])
            {
                case ' ':
                    if (InQuotes)
                    {
                        CurrentString += " ";
                        break;
                    }
                    Output.Add(CurrentString);
                    CurrentString = "";
                    break;
                case '"':
                    if (InQuotes)
                    {
                        InQuotes = false;
                        Output.Add(CurrentString);
                        CurrentString = "";
                        i += 1;
                        break;
                    }
                    InQuotes = true;
                    break;
                default:
                    CurrentString += line[i];
                    break;
            }
        }
        if (CurrentString != "")
        {
            Output.Add(CurrentString);
        }
        return Output.ToArray();
    }

    public static string GetComplement(string DNA)
    {
        StringBuilder Complement = new StringBuilder("", DNA.Length);
        foreach (char c in DNA.ToLower())
        {
            switch (c)
            {
                case 'a':
                    Complement.Append("t");
                    break;
                case 't':
                    Complement.Append("a");
                    break;
                case 'g':
                    Complement.Append("c");
                    break;
                case 'c':
                    Complement.Append("g");
                    break;
                case 'u':
                    Complement.Append('a');
                    break;
                default:
                    HelperFunctions.WriteError($"Error GIL07: Illegal character '{c}' in complement");
                    break;
            }
        }
        return Complement.ToString();
    }

    public static (Token[], Params) GetParams(List<Token> Tokens)
    {
        return GetParams(Tokens.ToArray());
    }

    public static (Token[], Params) GetParams(Token[] Tokens)
    {
        List<Token> OutputTokens = new List<Token>();
        Params OutputParams = new Params();

        foreach (Token token in Tokens)
        {
            switch (token.TokenType)
            {
                case LexerTokens.PARAM:
                    string[] Splitted = token.Value.Split(':');
                    OutputParams.Add(Splitted[0], Splitted[1]);
                    break;
                default:
                    OutputTokens.Add(token);
                    break;
            }
        }
        return (OutputTokens.ToArray(), OutputParams);
    }

    public static string GetAminos(Token[] Tokens, bool Unsafe = false)
    {
        string Output = "";
        foreach (Token t in Tokens)
        {
            if (t.TokenType == LexerTokens.IDENT)
            {
                if (!Unsafe)
                {
                    VerifyAminos(t.Value);
                }
                Output += t.Value;
            }
        }
        return Output;
    }
    
    public static void VerifyAminos(string Sequence)
    {
        string IncorrectChars = "";
        foreach (char c in Sequence)
        {
            if (!AminoLetters.Contains(c))
            {
                IncorrectChars += $"{c} ";
            }
        }

        if (IncorrectChars != "")
        {
            WriteError($"Error GIL11: Sequence contained the following non amino acid characters:\n{IncorrectChars}");
        }
    }





    public static List<char> AminoLetters = new List<char>(){
        'g', 'a', 'v', 'l', 'i', 'm', 'p', 'f', 'w', 's', 't', 'n', 'q', 'y', 'c', 'k', 'r', 'h', 'd', 'e', 'x',
        'G', 'A', 'V', 'L', 'I', 'M', 'P', 'F', 'W', 'S', 'T', 'N', 'Q', 'Y', 'C', 'K', 'R', 'H', 'D', 'E', 'X'
    };
}

public class Params
{
    private Dictionary<string, string> Param2Value = new Dictionary<string, string>();

    public void Add(string Param, string Value)
    {
        if (Param2Value.ContainsKey(Param))
        {
            Param2Value[Param] = Value;
        } else
        {
            Param2Value.Add(Param, Value);
        }
    }

    public string Get(string Param, bool ThrowOnFail = false, string ErrorMessage = "")
    {
        if (Param2Value.ContainsKey(Param))
        {
            return Param2Value[Param];
        }
        if (ThrowOnFail)
        {
            if (ErrorMessage == "")
            {
                HelperFunctions.WriteError($"You must provide a value for parameter \"{Param}\"");
            } else
            {
                HelperFunctions.WriteError(ErrorMessage);
            }
        }
        return "";
    }
}