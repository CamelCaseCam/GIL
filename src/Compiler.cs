using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class Compiler
{
    public void Compile(string path)
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
        string code = "";

        for (int i = 0; i < CurrentProject.Tokens.Count; i++)
        {
            Token CurrentToken = CurrentProject.Tokens[i];
            switch (CurrentToken.TokenType)
            {
                case LexerTokens.CODON:
                    Console.WriteLine(CurrentToken.Value);
                    code += CurrentToken.Value;
                    break;
                case LexerTokens.AMINOCODE:
                    if (Char.IsNumber(CurrentToken.Value[CurrentToken.Value.Length - 1]))
                    {
                        Console.WriteLine("Worked");
                    }
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
}