using System;
using System.Text;

public static class CallOp
{
    public static (Feature[], string) Call(RelativeFeature[] Sequences, GBSequence InnerCode, int pos)
    {
        return Call(Sequences, new RelativeFeature(InnerCode), pos);
    }

    public static (Feature[], string) Call(RelativeFeature[] Sequences, RelativeFeature InnerCode, int pos)
    {
        (int FeatureCount, int CodeLen) = GetEndLength(Sequences, InnerCode);
        StringBuilder OutputCode = new StringBuilder("", CodeLen);
        Feature[] Features = new Feature[FeatureCount];

        int FeatureStart = 0;
        Feature[] f;
        string dna;
        for (int i = 1; i < Sequences.Length; i++)
        {
            (f, dna) = Sequences[i - 1].Get(OutputCode.Length + pos);
            OutputCode.Append(dna);
            for (int x = 0; x < f.Length; x++)
            {
                Features[FeatureStart + x] = f[x];
            }
            FeatureStart += f.Length;

            //Now the InnerCode
            (f, dna) = InnerCode.Get(OutputCode.Length + pos);
            OutputCode.Append(dna);
            for (int x = 0; x < f.Length; x++)
            {
                Features[FeatureStart + x] = f[x];
            }
            FeatureStart += f.Length;
        }
        (f, dna) = Sequences[Sequences.Length - 1].Get(OutputCode.Length + pos);
        OutputCode.Append(dna);
        for (int i = 0; i < f.Length; i++)
        {
            Features[FeatureStart + i] = f[i];
        }
        FeatureStart += f.Length;

        return (Features, OutputCode.ToString());
    }

    public static (int, int) GetEndLength(RelativeFeature[] Sequences, RelativeFeature InnerCode)
    {
        int FeatureCount = InnerCode.Features.Length * Sequences.Length - 1;
        int CodeLen = InnerCode.DNA.Length * Sequences.Length - 1;
        
        if (FeatureCount < 0)
        {
            FeatureCount = 0;
        }
        if (CodeLen < 0)
        {
            CodeLen = 0;
        }

        foreach (RelativeFeature feature in Sequences)
        {
            FeatureCount += feature.Features.Length;
            CodeLen += feature.DNA.Length;
        }
        return (FeatureCount, CodeLen);
    }
}