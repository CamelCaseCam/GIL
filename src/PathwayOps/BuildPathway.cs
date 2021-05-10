using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public static class PathwayBuilder
{
    static Pathway Output;

    public static void BuildPathway(string path)
    {
        Output = new Pathway();

        string text = File.ReadAllText(path);    //Clean input and get lines
        text = text.Replace("\r", "");
        string[] lines = text.Split('\n');

        List<string> GeneDefs = new List<string>();
        List<string> ReactionDefs = new List<string>();
        List<string> PathwayDefs = new List<string>();
        List<string> SpontaneusDefs = new List<string>();
        foreach (string line in lines)    //order lines by execution order
        {
            if (line.Length < 3 || line[0] != '%') 
            {
                continue;
            }

            if (line[1] == 'g')
            {
                GeneDefs.Add(line.Substring(2, line.Length - 2));
            } else if (line[1] == 'r')
            {
                ReactionDefs.Add(line.Substring(2, line.Length - 2));
            } else if (line[1] == 'p')
            {
                PathwayDefs.Add(line.Substring(2, line.Length - 2));
            } else if (line[1] == 's')
            {
                SpontaneusDefs.Add(line.Substring(2, line.Length - 2));
            }
        }

        foreach (string line in GeneDefs)
        {
            DefineGene(line);
        }

        foreach (string line in ReactionDefs)
        {
            DefineReaction(line);
        }

        foreach (string line in PathwayDefs)
        {
            DefinePathway(line);
        }

        foreach (string line in SpontaneusDefs)
        {
            DefineSpontaneusReaction(line);
        }

        FileStream fs = new FileStream("Pathway", FileMode.Create);
        BinaryFormatter Formatter = new BinaryFormatter();
        try 
        {
            Formatter.Serialize(fs, Output);
        } catch (Exception e)
        {
            fs.Close();
            HelperFunctions.WriteError("Error serializing compiled pathway. Add an issue on the GitHub page with this error\n" +
            e.ToString());
        }
        fs.Close();
    }

    static void DefineGene(string line)
    {
        string[] ops = line.Split('|');    //Split line into gene name and code seperated by |
        Output.Genes.Add(ops[0], new Gene(ops[0], ops[1]));
    }

    static void DefineReaction(string line)
    {
        string[] ops = line.Split('|');
        string[] Genes = ops[0].Split(",");

        if (ops[1].Contains("<=>"))
        {
            string[] reaction = ops[1].Split("<=>");
            Reaction r = new Reaction(reaction[0].Split(","), Genes, reaction[1].Split(","), Output);
            Reaction r2 = new Reaction(reaction[1].Split(","), Genes, reaction[0].Split(","), Output);
            Output.Reactions.Add(r);

            foreach (string s in r.Reactants)
            {
                if (Output.Reactants2Genes.ContainsKey(s))
                {
                    string[] OldGenes = Output.Reactants2Genes[s];
                    string[] NewGenes = new string[OldGenes.Length + Genes.Length];
                    
                    for (int i = 0; i < OldGenes.Length; i++)
                    {
                        NewGenes[i] = OldGenes[i];
                    }

                    for (int i = 0; i < Genes.Length; i++)
                    {
                        NewGenes[i + OldGenes.Length] = Genes[i];
                    }
                    Output.Reactants2Genes[s] = NewGenes;
                    continue;
                }
                Output.Reactants2Genes.Add(s, Genes);
            }

            foreach (string s in r.Products)
            {
                if (Output.Products2Genes.ContainsKey(s))
                {
                    string[] OldGenes = Output.Products2Genes[s];
                    string[] NewGenes = new string[OldGenes.Length + Genes.Length];
                    
                    for (int i = 0; i < OldGenes.Length; i++)
                    {
                        NewGenes[i] = OldGenes[i];
                    }

                    for (int i = 0; i < Genes.Length; i++)
                    {
                        NewGenes[i + OldGenes.Length] = Genes[i];
                    }
                    Output.Products2Genes[s] = NewGenes;
                    continue;
                }
                Output.Products2Genes.Add(s, Genes);
            }

            foreach (string s in r.Products)
            {
                if (Output.Products2Reactions.ContainsKey(s))
                {
                    int[] OldReactions = Output.Products2Reactions[s];
                    int[] NewReactions = new int[OldReactions.Length + 1];
                    
                    for (int i = 0; i < OldReactions.Length; i++)
                    {
                        NewReactions[i] = OldReactions[i];
                    }
                    NewReactions[OldReactions.Length] = Output.Reactions.Count - 1;
                    Output.Products2Reactions[s] = NewReactions;
                    continue;
                }
                Output.Products2Reactions.Add(s, new int[] {Output.Reactions.Count - 1});
            }


            //Reaction 2
            Output.Reactions.Add(r2);
            foreach (string s in r2.Reactants)
            {
                if (Output.Reactants2Genes.ContainsKey(s))
                {
                    string[] OldGenes = Output.Reactants2Genes[s];
                    string[] NewGenes = new string[OldGenes.Length + Genes.Length];
                    
                    for (int i = 0; i < OldGenes.Length; i++)
                    {
                        NewGenes[i] = OldGenes[i];
                    }

                    for (int i = 0; i < Genes.Length; i++)
                    {
                        NewGenes[i + OldGenes.Length] = Genes[i];
                    }
                    Output.Reactants2Genes[s] = NewGenes;
                    continue;
                }
                Output.Reactants2Genes.Add(s, Genes);
            }

            foreach (string s in r2.Products)
            {
                if (Output.Products2Genes.ContainsKey(s))
                {
                    string[] OldGenes = Output.Products2Genes[s];
                    string[] NewGenes = new string[OldGenes.Length + Genes.Length];
                    
                    for (int i = 0; i < OldGenes.Length; i++)
                    {
                        NewGenes[i] = OldGenes[i];
                    }

                    for (int i = 0; i < Genes.Length; i++)
                    {
                        NewGenes[i + OldGenes.Length] = Genes[i];
                    }
                    Output.Products2Genes[s] = NewGenes;
                    continue;
                }
                Output.Products2Genes.Add(s, Genes);
            }

            foreach (string s in r2.Products)
            {
                if (Output.Products2Reactions.ContainsKey(s))
                {
                    int[] OldReactions = Output.Products2Reactions[s];
                    int[] NewReactions = new int[OldReactions.Length + 1];
                    
                    for (int i = 0; i < OldReactions.Length; i++)
                    {
                        NewReactions[i] = OldReactions[i];
                    }
                    NewReactions[OldReactions.Length] = Output.Reactions.Count - 1;
                    Output.Products2Reactions[s] = NewReactions;
                    continue;
                }
                Output.Products2Reactions.Add(s, new int[] {Output.Reactions.Count - 1});
            }
        } else
        {
            string[] reaction = ops[1].Split("=>");
            Reaction r = new Reaction(reaction[0].Split(","), Genes, reaction[1].Split(","), Output);
            Output.Reactions.Add(r);

            foreach (string s in r.Reactants)
            {
                if (Output.Reactants2Genes.ContainsKey(s))
                {
                    string[] OldGenes = Output.Reactants2Genes[s];
                    string[] NewGenes = new string[OldGenes.Length + Genes.Length];
                    
                    for (int i = 0; i < OldGenes.Length; i++)
                    {
                        NewGenes[i] = OldGenes[i];
                    }

                    for (int i = 0; i < Genes.Length; i++)
                    {
                        NewGenes[i + OldGenes.Length] = Genes[i];
                    }
                    Output.Reactants2Genes[s] = NewGenes;
                    continue;
                }
                Output.Reactants2Genes.Add(s, Genes);
            }

            foreach (string s in r.Products)
            {
                if (Output.Products2Genes.ContainsKey(s))
                {
                    string[] OldGenes = Output.Products2Genes[s];
                    string[] NewGenes = new string[OldGenes.Length + Genes.Length];
                    
                    for (int i = 0; i < OldGenes.Length; i++)
                    {
                        NewGenes[i] = OldGenes[i];
                    }

                    for (int i = 0; i < Genes.Length; i++)
                    {
                        NewGenes[i + OldGenes.Length] = Genes[i];
                    }
                    Output.Products2Genes[s] = NewGenes;
                    continue;
                }
                Output.Products2Genes.Add(s, Genes);
            }

            foreach (string s in r.Products)
            {
                if (Output.Products2Reactions.ContainsKey(s))
                {
                    int[] OldReactions = Output.Products2Reactions[s];
                    int[] NewReactions = new int[OldReactions.Length + 1];
                    
                    for (int i = 0; i < OldReactions.Length; i++)
                    {
                        NewReactions[i] = OldReactions[i];
                    }
                    NewReactions[OldReactions.Length] = Output.Reactions.Count - 1;
                    Output.Products2Reactions[s] = NewReactions;
                    continue;
                }
                Output.Products2Reactions.Add(s, new int[] {Output.Reactions.Count - 1});
            }
        }
    }

    static void DefinePathway(string line)
    {
        string[] ops = line.Split('|');
        string[] Genes = ops[1].Split(", ");
        Gene[] UsedGenes = new Gene[Genes.Length];
        
        for (int i = 0; i < Genes.Length; i++)
        {
            UsedGenes[i] = Output.Genes[Genes[i]];
        }
        Output.Pathways.Add(ops[0], UsedGenes);
    }

    static void DefineSpontaneusReaction(string line)
    {
        string[] ops = line.Split('|');
        string[] Genes = new string[] {""};

        if (line.Contains("<=>"))
        {
            string[] reaction = line.Split("<=>");
            Reaction r = new Reaction(reaction[0].Split(","), Genes, reaction[1].Split(","), Output);
            Reaction r2 = new Reaction(reaction[1].Split(","), Genes, reaction[0].Split(","), Output);
            Output.Reactions.Add(r);

            foreach (string s in r.Reactants)
            {
                if (Output.Reactants2Genes.ContainsKey(s))
                {
                    string[] OldGenes = Output.Reactants2Genes[s];
                    string[] NewGenes = new string[OldGenes.Length + Genes.Length];
                    
                    for (int i = 0; i < OldGenes.Length; i++)
                    {
                        NewGenes[i] = OldGenes[i];
                    }

                    for (int i = 0; i < Genes.Length; i++)
                    {
                        NewGenes[i + OldGenes.Length] = Genes[i];
                    }
                    Output.Reactants2Genes[s] = NewGenes;
                    continue;
                }
                Output.Reactants2Genes.Add(s, Genes);
            }

            foreach (string s in r.Products)
            {
                if (Output.Products2Genes.ContainsKey(s))
                {
                    string[] OldGenes = Output.Products2Genes[s];
                    string[] NewGenes = new string[OldGenes.Length + Genes.Length];
                    
                    for (int i = 0; i < OldGenes.Length; i++)
                    {
                        NewGenes[i] = OldGenes[i];
                    }

                    for (int i = 0; i < Genes.Length; i++)
                    {
                        NewGenes[i + OldGenes.Length] = Genes[i];
                    }
                    Output.Products2Genes[s] = NewGenes;
                    continue;
                }
                Output.Products2Genes.Add(s, Genes);
            }

            foreach (string s in r.Products)
            {
                if (Output.Products2Reactions.ContainsKey(s))
                {
                    int[] OldReactions = Output.Products2Reactions[s];
                    int[] NewReactions = new int[OldReactions.Length + 1];
                    
                    for (int i = 0; i < OldReactions.Length; i++)
                    {
                        NewReactions[i] = OldReactions[i];
                    }
                    NewReactions[OldReactions.Length] = Output.Reactions.Count - 1;
                    Output.Products2Reactions[s] = NewReactions;
                    continue;
                }
                Output.Products2Reactions.Add(s, new int[] {Output.Reactions.Count - 1});
            }


            //Reaction 2
            Output.Reactions.Add(r2);
            foreach (string s in r2.Reactants)
            {
                if (Output.Reactants2Genes.ContainsKey(s))
                {
                    string[] OldGenes = Output.Reactants2Genes[s];
                    string[] NewGenes = new string[OldGenes.Length + Genes.Length];
                    
                    for (int i = 0; i < OldGenes.Length; i++)
                    {
                        NewGenes[i] = OldGenes[i];
                    }

                    for (int i = 0; i < Genes.Length; i++)
                    {
                        NewGenes[i + OldGenes.Length] = Genes[i];
                    }
                    Output.Reactants2Genes[s] = NewGenes;
                    continue;
                }
                Output.Reactants2Genes.Add(s, Genes);
            }

            foreach (string s in r2.Products)
            {
                if (Output.Products2Genes.ContainsKey(s))
                {
                    string[] OldGenes = Output.Products2Genes[s];
                    string[] NewGenes = new string[OldGenes.Length + Genes.Length];
                    
                    for (int i = 0; i < OldGenes.Length; i++)
                    {
                        NewGenes[i] = OldGenes[i];
                    }

                    for (int i = 0; i < Genes.Length; i++)
                    {
                        NewGenes[i + OldGenes.Length] = Genes[i];
                    }
                    Output.Products2Genes[s] = NewGenes;
                    continue;
                }
                Output.Products2Genes.Add(s, Genes);
            }

            foreach (string s in r2.Products)
            {
                if (Output.Products2Reactions.ContainsKey(s))
                {
                    int[] OldReactions = Output.Products2Reactions[s];
                    int[] NewReactions = new int[OldReactions.Length + 1];
                    
                    for (int i = 0; i < OldReactions.Length; i++)
                    {
                        NewReactions[i] = OldReactions[i];
                    }
                    NewReactions[OldReactions.Length] = Output.Reactions.Count - 1;
                    Output.Products2Reactions[s] = NewReactions;
                    continue;
                }
                Output.Products2Reactions.Add(s, new int[] {Output.Reactions.Count - 1});
            }
        } else
        {
            string[] reaction = line.Split("=>");
            Reaction r = new Reaction(reaction[0].Split(","), Genes, reaction[1].Split(","), Output);
            Output.Reactions.Add(r);

            foreach (string s in r.Reactants)
            {
                if (Output.Reactants2Genes.ContainsKey(s))
                {
                    string[] OldGenes = Output.Reactants2Genes[s];
                    string[] NewGenes = new string[OldGenes.Length + Genes.Length];
                    
                    for (int i = 0; i < OldGenes.Length; i++)
                    {
                        NewGenes[i] = OldGenes[i];
                    }

                    for (int i = 0; i < Genes.Length; i++)
                    {
                        NewGenes[i + OldGenes.Length] = Genes[i];
                    }
                    Output.Reactants2Genes[s] = NewGenes;
                    continue;
                }
                Output.Reactants2Genes.Add(s, Genes);
            }

            foreach (string s in r.Products)
            {
                if (Output.Products2Genes.ContainsKey(s))
                {
                    string[] OldGenes = Output.Products2Genes[s];
                    string[] NewGenes = new string[OldGenes.Length + Genes.Length];
                    
                    for (int i = 0; i < OldGenes.Length; i++)
                    {
                        NewGenes[i] = OldGenes[i];
                    }

                    for (int i = 0; i < Genes.Length; i++)
                    {
                        NewGenes[i + OldGenes.Length] = Genes[i];
                    }
                    Output.Products2Genes[s] = NewGenes;
                    continue;
                }
                Output.Products2Genes.Add(s, Genes);
            }

            foreach (string s in r.Products)
            {
                if (Output.Products2Reactions.ContainsKey(s))
                {
                    int[] OldReactions = Output.Products2Reactions[s];
                    int[] NewReactions = new int[OldReactions.Length + 1];
                    
                    for (int i = 0; i < OldReactions.Length; i++)
                    {
                        NewReactions[i] = OldReactions[i];
                    }
                    NewReactions[OldReactions.Length] = Output.Reactions.Count - 1;
                    Output.Products2Reactions[s] = NewReactions;
                    continue;
                }
                Output.Products2Reactions.Add(s, new int[] {Output.Reactions.Count - 1});
            }
        }
    }
}

[Serializable]
public class Pathway
{
    public Dictionary<string, Gene> Genes = new Dictionary<string, Gene>();
    public List<Reaction> Reactions = new List<Reaction>();
    public Dictionary<string, string[]> Products2Genes = new Dictionary<string, string[]>();
    public Dictionary<string, string[]> Reactants2Genes = new Dictionary<string, string[]>();
    public Dictionary<string, int[]> Products2Reactions = new Dictionary<string, int[]>();
    public Dictionary<string, Gene[]> Pathways = new Dictionary<string, Gene[]>();
}

[Serializable]
public class Reaction
{
    public string[] Reactants;
    public Gene[] Enzymes;
    public string[] Products;

    public Reaction(string[] Reactants, string[] Genes, string[] Products, Pathway Context)
    {
        this.Reactants = Reactants;    //set reactants and products
        this.Products = Products;
        this.Enzymes = new Gene[Genes.Length];    //create array of length of genes
        if (Genes[0] == "")
        {
            Enzymes = new Gene[] {null};
            return;
        }
        for (int i = 0; i < Genes.Length; i++)
        {
            this.Enzymes[i] = Context.Genes[Genes[i]];    //Get ith gene
        }
    }
}

[Serializable]
public class Gene
{
    public Gene(string Name, string Code)
    {
        this.Name = Name;
        this.Code = Code;
    }

    public string Name;
    public string Code;
}