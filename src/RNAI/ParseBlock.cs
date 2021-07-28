using System;
using System.Collections;
using System.Collections.Generic;

public static class ParseBlock
{
    public static DNABlock[] Parse(List<Token> Tokens, Project CurrentProject)
    {
        List<DNABlock> Output = new List<DNABlock>();
        DNABlock CurrentBlock = new DNABlock("add", "");
        List<Token> InnerCode = new List<Token>();
        Project Inside;
        
        for (int i = 0; i < Tokens.Count; i++)
        {
            if (Tokens[i].TokenType == LexerTokens.AND || Tokens[i].TokenType == LexerTokens.NOT)
            {
                Inside = CurrentProject.Copy();
                Inside.Tokens = InnerCode;
                CurrentBlock.DNA = Compiler.ExecutingCompiler.CompileGB(Inside, "").Bases.ToLower();
                Output.Add(CurrentBlock);
                InnerCode = new List<Token>();
                CurrentBlock = new DNABlock(Tokens[i].Value, "");
            } else
            {
                InnerCode.Add(Tokens[i]);
            }
        }
        
        Inside = CurrentProject.Copy();
        Inside.Tokens = InnerCode;
        CurrentBlock.DNA = Compiler.ExecutingCompiler.CompileGB(Inside, "").Bases.ToLower();
        Output.Add(CurrentBlock);
        return Output.ToArray();
    }
}

public class DNABlock
{
    public string BlockType;    //add or sub
    public string DNA;

    public DNABlock(string type, string DNA)
    {
        if (type == "&")
        {
            type = "add";
        } else if (type == "!")
        {
            type = "sub";
        }
        BlockType = type;
        this.DNA = DNA.Replace('u', 't').Replace('U', 'T');
    }

    public override string ToString()
    {
        return BlockType;
    }
}