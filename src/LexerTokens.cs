using System.Collections;
using System.Collections.Generic;

public static class LexerTokens
{
    //Meta-tokens
    //public const string IMPORT = "IMPORT";    //import other projects
    public const string IDENT = "IDENT";
    public const string NEWLINE = "NEWLINE";
    public const string COMMENT = "COMMENT";
    public const string ENTRYPOINT = "ENTRYPOINT";

    //Metadata
    public const string SETTARGET = "SETTARGET";
    public const string BEGINREGION = "BEGINREGION";    //#region Name
    public const string ENDREGION = "ENDREGION";    //#EndRegion

    //DNA
    public const string AMINOLETTER = "AMINOLETTER";    //single-letter representation of amino acid
    public const string AMINOCODE = "AMINOCODE";    //three-letter code for an amino acid
    public const string CODON = "CODON";

    public const string BEGIN = "BEGIN";
    public const string END = "END";

    public const string AMINOSEQUENCE = "AMINOSEQUENCE";

    public static readonly RegexLexer Lexer = new RegexLexer(
        new (string, string)[] {    //you can change these to translate GIL into different languages
            (@"//.*", COMMENT),
            (@"(?<=/\*)[\s\S\n]+?(?=\*/)", COMMENT),
            (@"(?<=#target )[a-zA-Z]*", SETTARGET),
            (@"(?<=#Target )[a-zA-Z]*", SETTARGET),
            (@"(?<=#region )[a-zA-Z]*", BEGINREGION),
            (@"(?<=#Region )[a-zA-Z]*", BEGINREGION),
            (@"#endRegion", ENDREGION),
            (@"#EndRegion", ENDREGION),
            (@"(?<=#)EntryPoint", ENTRYPOINT),
            //(@"(?<=import )[\S]*", IMPORT),
            (@"(?<=%AminoSequence\{)[\w\W]*(?=\})", AMINOSEQUENCE),

            (@"\{", BEGIN),
            (@"\}", END),
            (@"[ATCGatcg]{3}", CODON),
            (@"(?<= )(gly|ala|val|leu|ile|met|pro|phe|trp|ser|thr|asn|gln|tyr|cys|lys|arg|his|asp|glu)(?= |\n)", AMINOCODE),
            (@"(?<=\W)[g|a|v|l|i|m|p|f|w|s|t|n|q|y|c|k|r|h|d|e](?=\W)", AMINOLETTER),
            (@"\n", NEWLINE),
            
            //must be last token so it's checked last
            (@"(?<![\.@])(?<=\s)[a-zA-Z]{4,}", IDENT),
        }
    );
}

public class Token
{
    public Token(string type, string val)
    {
        TokenType = type;
        Value = val;
    }
    public string TokenType;
    public string Value;

    public override string ToString()
    {
        return $"\"{TokenType}, {Value}\"";
    }
}