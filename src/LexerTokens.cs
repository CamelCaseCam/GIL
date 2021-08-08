using System.Collections;
using System.Collections.Generic;

public static class LexerTokens
{
    public static readonly List<string> ReservedNames = new List<string>() {
        "AminoSequence",
        "sequence",
        "operation",
        "import", 
        "using",
        "From",
        "For",
        "Block",
    };

    //Meta-tokens
    public const string IMPORT = "IMPORT";    //import other projects
    public const string USING = "USING";    //link to outside dlls
    public const string IDENT = "IDENT";
    public const string NEWLINE = "NEWLINE";
    public const string COMMENT = "COMMENT";
    public const string ENTRYPOINT = "ENTRYPOINT";
    public const string SETATTR = "SETATTR";
    public const string PARAM = "PARAM";

    //Metadata
    public const string SETTARGET = "SETTARGET";
    public const string BEGINREGION = "BEGINREGION";    //#region Name
    public const string ENDREGION = "ENDREGION";    //#EndRegion
    public const string REFREGION = "REFREGION";

    //DNA
    public const string AMINOLETTER = "AMINOLETTER";    //single-letter representation of amino acid
    public const string AMINOCODE = "AMINOCODE";    //three-letter code for an amino acid
    public const string CODON = "CODON";

    public const string BEGIN = "BEGIN";
    public const string END = "END";

    public const string AMINOSEQUENCE = "AMINOSEQUENCE";
    public const string FROM = "FROM";
    public const string FOR = "FOR";

    public const string DEFINESEQUENCE = "DEFINESEQUENCE";
    public const string DEFOP = "DEFOP";
    public const string INNERCODE = "INNERCODE";
    public const string CALLOP = "CALLOP";

    //RNAI
    public const string BLOCK = "BLOCK";
    public const string AND = "AND";
    public const string NOT = "NOT";


    public static readonly RegexLexer Lexer = new RegexLexer(
        new (string, string)[] {    //you can change these to translate GIL into different languages
            (@"//[^\n]*", COMMENT),
            (@"(?<=/\*)[\s\S\n]+?(?=\*/)", COMMENT),
            (@"(?<=#target )[a-zA-Z]*", SETTARGET),
            (@"(?<=#Target )[a-zA-Z]*", SETTARGET),
            (@"(?<=#region )[a-zA-Z0-9_ ]*", BEGINREGION),
            (@"(?<=#Region )[a-zA-Z0-9_ ]*", BEGINREGION),
            (@"(?<=#endRegion)[a-zA-Z0-9_ ]*", ENDREGION),
            (@"(?<=#EndRegion)[a-zA-Z0-9_ ]*", ENDREGION),
            (@"(?<=#)EntryPoint", ENTRYPOINT),
            (@"(?<=#SetAttribute )[a-zA-Z0-9_-]{1,}:[a-zA-Z0-9\._-]{1,}", SETATTR),
            (@"(?<=#setAttribute )[a-zA-Z0-9_-]{1,}:[a-zA-Z0-9\._-]{1,}", SETATTR),
            (@"(?<=#SetAtr )[a-zA-Z0-9_-]{1,}:[a-zA-Z0-9\._-]{1,}", SETATTR),
            (@"(?<=\W)(?<!#SetAttribute )(?<!#setAttribute )(?<!#SetAtr )[a-zA-Z0-9_-]{1,}:[a-zA-Z0-9_\-/\\]*", PARAM),
            (@"(?<=sequence )[a-zA-Z0-9@*_-]*", DEFINESEQUENCE),
            (@"(?<=operation )[a-zA-Z0-9@*_-]*", DEFOP),
            (@"(?<=\$)innerCode", INNERCODE),
            (@"(?<=\$)InnerCode", INNERCODE),
            (@"(?<=\.)[a-zA-Z0-9@*_-]*", CALLOP),
            (@"(?<=import ).*", IMPORT),
            (@"(?<=using ).*", USING),
            (@"(?<=AminoSequence \{)[^\}]*(?=\})", AMINOSEQUENCE),
            (@"(?<=AminoSequence\n\{)[^\}]*(?=\})", AMINOSEQUENCE),
            (@"(?<=From )[a-zA-Z]*", FROM),
            (@"(?<=From )[a-zA-Z]*", FROM),
            (@"(?<=For )[a-zA-Z]*", FOR),
            (@"(?<=For )[a-zA-Z]*", FOR),
            (@"Block", BLOCK),
            (@"&", AND),
            (@"!", NOT),

            (@"\{", BEGIN),
            (@"\}", END),
            (@"(?<=\s|\n)[ATCGatcg^!^&]{1,}(?=\s|\n)", CODON),
            (@"(?<=\s|\n|!|&)[ATUCGatucg^!^&]{1,}(?=\s|\n)", CODON),
            (@"(?=\s|\n)(gly|ala|val|leu|ile|met|pro|phe|trp|ser|thr|asn|gln|tyr|cys|lys|arg|his|asp|glu)(?<=\s|\n)", AMINOCODE),
            (@"(?<=\W)[g|a|v|l|i|m|p|f|w|s|t|n|q|y|c|k|r|h|d|e|x](?=\W)", AMINOLETTER),
            (@"\n", NEWLINE),
            
            //must be last token so it's checked last
            (@"(?<![\.@])(?<=\s|\n)[a-zA-Z0-9@*_-]{4,}", IDENT),
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