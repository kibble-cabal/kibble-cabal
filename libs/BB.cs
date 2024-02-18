using Godot;

namespace BB
{
    public static class BB
    {
        public static string Code(this string str) => $"[code]{str}[/code]";
        public static string Color(this string str, string color) => $"[color={color}]{str}[/color]";
        public static string Color(this string str, Color color) => str.Color(color.ToHtml());
        public static string Gray(this string str) => str.Color("gray");
        public static string White(this string str) => str.Color("white");
        public static string Red(this string str) => str.Color("red");
        public static string Yellow(this string str) => str.Color("yellow");
        public static string Green(this string str) => str.Color("green");
        public static string Blue(this string str) => str.Color("blue");
        public static string Purple(this string str) => str.Color("purple");
        public static string Pink(this string str) => str.Color("pink");
        public static string Magenta(this string str) => str.Color("magenta");
        public static string Image(this string path) => $"[img]{path}[/img]";
        public static string URL(this string path, string? str = null) => str != null ? $"[url={path}]{str}[/url]" : $"[url={path}]{path}[/url]";
    }
}