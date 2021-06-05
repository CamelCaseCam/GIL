using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class PathwayOps
{
    public static int MaxDepth = 10;
    public static void Repress(string name, Pathway pathway)
    {
        Console.WriteLine(TraverseRepress(name, pathway, 0));
    }

    static Gene TraverseRepress(string Product, Pathway p, int Depth, int Last = -1, bool JoiningPoint = false)
    {
        Reaction r = null;
        bool CheckIfTwoWay = true;
        if (!p.Products2Reactions.ContainsKey(Product))    //If the compound isn't made in the pathway, throw an error
        {
            HelperFunctions.WriteError($"No reaction making product \"{Product}\" found in current pathway");
        }

        int[] Reactions = p.Products2Reactions[Product];    //Get reactions that make product
        
        Redo:
        if (Reactions.Length == 1)    //if there's only one reaction
        {
            r = p.Reactions[Reactions[0]];    //Get reaction
            if (Depth == MaxDepth)    //if maximum depth has been reached, return current gene
            {
                goto ReturnCurrentGene;
            }

            /*if current reaction makes more than one thing and it isn't the joining point between two 
            branches, return null*/
            if (r.Products.Length > 1 && !JoiningPoint)
            {
                return null;
            }

            //NEEDS TO BE REWORKED, I know this is redundant, but in the next release it'll follow each path
            if (r.Reactants.Length > 1)    //if there's more than 1 reactant, follow the first one
            {
                Gene g = TraverseRepress(r.Reactants[0], p, Depth + 1, Last: Reactions[0]);
                if (g == null)
                {
                    goto ReturnCurrentGene;
                }
                return g;
            } else
            {
                Gene g = TraverseRepress(r.Reactants[0], p, Depth + 1, Last: Reactions[0]);
                if (g == null)
                {
                    goto ReturnCurrentGene;
                }
                return g;
            }
        } else if (Reactions.Length == 2 && CheckIfTwoWay)    //Check if it's a two way reaction
        {
            if (Reactions[0] == Last)    //if the first reaction is the last one
            {
                Reactions = new int[] { Reactions[1] };
                goto Redo;
            }
            if (Reactions[1] == Last)    //if the second reaction is the last one
            {
                Reactions = new int[] { Reactions[0] };
                goto Redo;
            }
            CheckIfTwoWay = false;
            goto Redo;
        } else    //if there's more than one product
        {
            if (Depth == MaxDepth)    //if maximum depth has been reached, return last gene
            {
                return null;
            }

            int Overlap = GetOverlap("", p, Depth + 1, Last, Reactions.Length, BypassReaction: Reactions[0]);
            for (int i = 1; i < Reactions.Length; i++)    //for each branch, check if overlap is the same
            {
                if (GetOverlap("", p, Depth + 1, Last, Reactions.Length, BypassReaction: Reactions[i]) != Overlap)
                {
                    return null;
                }
            }
            r = p.Reactions[Overlap];

            //NEEDS TO BE REWORKED, I know this is redundant, but in the next release it'll follow each path
            if (r.Reactants.Length > 1)    //if there's more than 1 reactant, follow the first one
            {
                Gene g = TraverseRepress(r.Reactants[0], p, Depth + 1, Last: Overlap);
                if (g == null)
                {
                    goto ReturnCurrentGene;
                }
                return g;
            } else
            {
                Gene g = TraverseRepress(r.Reactants[0], p, Depth + 1, Last: Overlap);
                if (g == null)
                {
                    goto ReturnCurrentGene;
                }
                return g;
            }
        }

        ReturnCurrentGene:
        return r.Enzymes[0];
    }

    static int GetOverlap(string Product, Pathway p, int Depth, int Last, int NumBranches, int BypassReaction = -1)    //Function to get possible overlap
    {
        int[] Reactions = null;
        bool Bypass = false;
        if (BypassReaction != -1)
        {
            Reactions = new int[] { BypassReaction };
            Bypass = true;
            goto MoveUpwards;
        }

        if (!p.Products2Reactions.ContainsKey(Product))    //if product isn't made in pathway, return -1
        {
            return -1;
        }
        if (Depth == MaxDepth)    //If max depth has been reached, return -1
        {
            return -1;
        }

        Reactions = p.Products2Reactions[Product];    //Get product
        
        MoveUpwards:
        if (Reactions.Length == 1 || Bypass)    //if there's only one reaction, move upwards
        {
            Reaction r = p.Reactions[Reactions[0]];    //Get reaction

            if (r.Products.Length == NumBranches)
            {
                return Reactions[0];
            }

            if (r.Reactants.Length > 1)    //If multiple reactants
            {
                //Evaluate each reactant
                foreach (string s in r.Reactants)
                {
                    int i = GetOverlap(s, p, Depth + 1, Reactions[0], NumBranches);
                    if (i != -1)
                    {
                        return i;
                    }
                }

                return -1;
            }
            
            return GetOverlap(r.Reactants[0], p, Depth + 1, Reactions[0], NumBranches);
        } else if (Reactions.Length == 2)    //Check if it's a two way reaction
        {
            if (Reactions[0] == Last)    //if the first reaction is the last one
            {
                Reactions[0] = Reactions[1];
                Bypass = true;
                goto MoveUpwards;
            }
            if (Reactions[1] == Last)    //if the second reaction is the last one
            {
                Bypass = true;
                goto MoveUpwards;
            }
            return -1;
        }
        return -1;
    }
}