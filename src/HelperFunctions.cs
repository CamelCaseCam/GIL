using System;

public static class HelperFunctions
{
    public static void WriteWarning(string text)
    {
        var OriginalColour = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(text);
        Console.ForegroundColor = OriginalColour;
    }

    public static void WriteError(string text)
    {
        var OriginalColour = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(text);
        Console.ForegroundColor = OriginalColour;
        Environment.Exit(0);
    }
}