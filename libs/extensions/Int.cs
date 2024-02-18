using System.Linq;

public static class IntExtensions
{
    public static bool IsNotification(this int what, params long[] notifications) => notifications.Select(n => (int)n).Contains(what);
}