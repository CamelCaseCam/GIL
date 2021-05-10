using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public static class LoadPathway
{
    public static Pathway Load(string target)
    {
        Pathway Output = null;
        FileStream fs = new FileStream(GIL.Program.DataPath + $"\\CompilationTargets\\{target}\\pathway", FileMode.Open);
        
        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            Output = (Pathway) formatter.Deserialize(fs);
        } catch (Exception e)
        {
            fs.Close();
            HelperFunctions.WriteError($"Error deserializing pathway for orgainsim {target}. Check the " + 
            "pathway for errors and add an issue on the GitHub page with this error\n" +
            e.ToString());
        }
        fs.Close();
        return Output;
    }
}