

public class RelativeFeature
{
    public static readonly RelativeFeature Empty = new RelativeFeature(new Feature[] {}, "");
    public Feature[] Features;
    public string DNA;

    public RelativeFeature(GBSequence SequenceToCopy)
    {
        Features = SequenceToCopy.Features;
        DNA = SequenceToCopy.Bases;
    }

    public RelativeFeature(Feature[] Features, string DNA)
    {
        this.Features = Features;
        this.DNA = DNA;
    }

    public (Feature[], string) Get(int pos)
    {
        Feature[] OutputFeatures = Features;
        foreach (Feature f in OutputFeatures)
        {
            f.Beginning += pos;
            f.End += pos;
        }
        return (OutputFeatures, DNA);
    }

    public override string ToString()
    {
        string str = "";
        foreach (Feature f in Features)
        {
            str += f;
        }
        return str;
    }
}