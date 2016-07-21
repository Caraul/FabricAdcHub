namespace FabricAdcHub.Core.Utilites
{
    public static class StringExtensions
    {
        public static string Escape(this string data)
        {
            return data
                .Replace(@"\", @"\\")
                .Replace(" ", @"\s")
                .Replace("\n", @"\n");
        }

        public static string Unescape(this string data)
        {
            return data
                .Replace(@"\\", @"\")
                .Replace(@"\s", " ")
                .Replace(@"\n", "\n");
        }
    }
}
