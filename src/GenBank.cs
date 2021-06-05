using System;

public class GBSequence
{
    public Feature[] Features = new Feature[] {};
    public string Bases;
    public string Target;
    public string FileName;

    public override string ToString()
    {
        string Output = $"LOCUS                               {Bases.Length} bp  DNA linear\n" + 
                        $"DEFINITION  {FileName} compiled with GIL version {GIL.Program.Version} for " + 
                        $"target {Target}\n" + 
                        $"ACCESSION   \n" + 
                        $"SOURCE      \n" +
                        $"  ORGANISM  {Target}\n\n" + 
                         "FEATURES             Location/Qualifiers\n" + 
                        $"     source          1..{Bases.Length}\n" + 
                        $"                     /organism=\"{Target}\"\n";
        for (int i = 0; i < Features.Length; i++)
        {
            Output += Features[i].ToString();
        }
        Output += "\n" + GetBases() + "//";
        
        return Output;
    }

    string GetBases()
    {
        int CurrentBase = 0;
        string Output = "ORIGIN\n";

        while (CurrentBase < Bases.Length)
        {
            string num = (CurrentBase + 1).ToString();
            string line = "";
            for (int i = 0; i < 9 - num.Length; i++)
            {
                line += " ";
            }
            line += num;

            for (int i = 0; i < 6; i++)
            {
                line += " ";
                if (CurrentBase + 10 < Bases.Length)
                {
                    line += Bases.Substring(CurrentBase, 10);
                    CurrentBase += 10;
                } else
                {
                    int RemainingLength = Bases.Length - CurrentBase;
                    line += Bases.Substring(CurrentBase, RemainingLength);
                    CurrentBase = Bases.Length;
                    break;
                }
            }
            Output += line + "\n";
        }
        return Output;
    }
}

public class Feature
{
    public string Name;
    public int Beginning;
    public int End;

    public Feature(string Name, int Beginning, int End)
    {
        this.Name = Name;
        this.Beginning = Beginning;
        this.End = End;
    }

    public override string ToString()
    {
        return  $"     CDS             {Beginning}..{End}\n" + 
                $"                     /label=\"{Name}\"\n";
    }
}

//DEFINITION  Saccharomyces cerevisiae TCP1-beta gene, partial cds, and Axl2p