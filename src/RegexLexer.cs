using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

public class RegexLexer
{
    public RegexLexer((string, string)[] patterns)
    {
        foreach (var item in patterns)
        {
            Patterns.Add(new TokenPattern(item.Item1, item.Item2));
        }
    }
    public List<TokenPattern> Patterns = new List<TokenPattern>();

    public List<Token> OldTokenize(string text)    //Tokenize method without GBSequence specific code
    {
        int CurrentIdx = 0;
        List<Token> Output = new List<Token>();
        while (CurrentIdx < text.Length)
        {
            Match BestMatch = null;
            TokenPattern BestPattern = null;
            foreach (TokenPattern pattern in Patterns)
            {
                Match match = pattern.expression.Match(text, CurrentIdx);
                if (BestMatch == null && match.Success)
                {
                    BestMatch = match;
                    BestPattern = pattern;
                    continue;
                }
                if (match.Success && match.Index < BestMatch.Index)
                {
                    BestMatch = match;
                    BestPattern = pattern;
                }
            }
            if (BestMatch == null) { break; }

            /*//GIL specific code
            if (BestPattern.Value == LexerTokens.IMPORT)
            {
                string path = BestMatch.Value;
                if (path[0] == '.')
                {
                    path = Environment.CurrentDirectory + path.Substring(1, path.Length - 1);
                }
                text += File.ReadAllText(path);
                Output.Add(new Token("IMPORTED_FILE", "\n\n"));
                CurrentIdx += 2;
                continue;
            }*/

            Output.Add(new Token(BestPattern.Value, BestMatch.Value));
            CurrentIdx = BestMatch.Index + BestMatch.Length;
        }
        return Output;
    }

    public (List<Token>, List<string>) Tokenize(string text)    //Returns tokens and list of regions that are referenced by name
    {
        int CurrentIdx = 0;
        List<Token> Output = new List<Token>();
        List<string> ReferencedRegions = new List<string>();
        while (CurrentIdx < text.Length)
        {
            Match BestMatch = null;
            TokenPattern BestPattern = null;
            foreach (TokenPattern pattern in Patterns)
            {
                Match match = pattern.expression.Match(text, CurrentIdx);
                if (BestMatch == null && match.Success)
                {
                    BestMatch = match;
                    BestPattern = pattern;
                    continue;
                }
                if (match.Success && match.Index < BestMatch.Index)
                {
                    BestMatch = match;
                    BestPattern = pattern;
                }
            }
            if (BestMatch == null) { break; }

            if (BestPattern.Value == LexerTokens.ENDREGION && BestMatch.Value != "" && BestMatch.Value[0] != '#')
            {
                ReferencedRegions.Add(BestMatch.Value.Substring(1, BestMatch.Value.Length - 1));
                Output.Add(new Token(BestPattern.Value, BestMatch.Value.Substring(1, BestMatch.Value.Length - 1)));
                CurrentIdx = BestMatch.Index + BestMatch.Length;
                continue;
            }

            Output.Add(new Token(BestPattern.Value, BestMatch.Value));
            CurrentIdx = BestMatch.Index + BestMatch.Length;
            if (BestMatch.Length == 0)
            {
                CurrentIdx++;
            }
        }
        return (Output, ReferencedRegions);
    }
}

public class TokenPattern
{
    public TokenPattern(string exp, string val)
    {
        expression = new Regex(exp);
        Value = val;
    }
    public Regex expression;
    public string Value;
}