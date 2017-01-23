namespace Chalkable.Common
{
    public static class UrlTools
    {
        public static string UrlCombine(params string[] pathes)
        {
            var res = pathes[0];
            for (int i = 1; i < pathes.Length; i++)
            {
                res = res.TrimEnd('/');
                pathes[i] = pathes[i].TrimStart('/');
                res = $"{res}/{pathes[i]}";
            }
            return res;
        }
    }
}
