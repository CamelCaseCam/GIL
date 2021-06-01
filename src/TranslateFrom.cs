using System;
using System.Text;

public static class TranslateFrom
{
    public static string Translate(string DNA, string From)
    {
        string OldCode = DNA.ToLower();    //Convert sequence to lower case
        StringBuilder NewCode = new StringBuilder("", DNA.Length);    //Since we know the max length, we use a StringBuilder

        for (int i = 0; i < DNA.Length; i++)    //Loop through sequence until start codon (atg)
        {
            if (i > DNA.Length - 3)
            {
                NewCode.Append(OldCode[i]);
                continue;
            }
            if (OldCode[i] == 'a' && OldCode[i + 1] == 't' && OldCode[i + 2] == 'g')
            {
                string Output = "";
                (i, Output) = TranslateCodons(OldCode, i, new CodonEncoding(From));    //jump to letter after stop codon
                NewCode.Append(Output);
                continue;
            }
            NewCode.Append(OldCode[i]);    //If no start codon, just append the sequence to the output
        }
        return NewCode.ToString();
    }

    static (int, string) TranslateCodons(string DNA, int Pos, CodonEncoding From)    //translate protein sequence
    {
        StringBuilder Output = new StringBuilder("", DNA.Length - Pos);    //translated sequence will be no bigger than remaining letters in original sequence
        int i;    //We want to return i, so it's initialized outside of the loop
        for (i = Pos; i < DNA.Length - 2; i += 3)    //DNA.Length - 3 fails to translate the last codon
        {
            //Get codon and translate it to a tuple representing its relative availability. ex. atg = (m, 1)
            string Codon = DNA.Substring(i, 3).ToLower();
            var c = From.CodonToCode(Codon);

            if (Codon != "taa" && Codon != "tga" && Codon != "tag")    //if it isn't a stop codon, translate and continue with the loop
            {
                Output.Append(GIL.Program.CurrentEncoding.GetLetter(c.Item1, c.Item2));
                continue;
            }
            Output.Append(GIL.Program.CurrentEncoding.GetLetter(c.Item1, c.Item2));    //Otherwise, translate and break out of loop
            i += 2;
            break;
        }
        return (i, Output.ToString());    //I could do this instead of break, but I just don't feel like it
    }
}