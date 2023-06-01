using System.Text;

namespace Chat.Shared.Extensions;

public static class StringBuilderExtension
{
    public static void AppendWithColor(this StringBuilder sb, string text, ConsoleColor color = ConsoleColor.White)
    {
        Console.ForegroundColor = color;
        var modified = text;
        Console.ResetColor();
        
        sb.Append(modified);
    }
}