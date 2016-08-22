using System;
using System.Linq;
using Chalkable.Common.Exceptions;

namespace Chalkable.StiConnector
{
    public static class VersionHelper
    {
        public static void ValidateVersionFormat(string version)
        {
            if (!IsValidVersionFormat(version))
                throw new ChalkableException("Invalid version format. Version should have next format 0.0.0.0");
        }

        private static bool IsValidVersionFormat(this string version)
        {
            if (string.IsNullOrWhiteSpace(version)) return false;

            var versionParts = version.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            int number;
            return versionParts.Length == 4 && versionParts.All(x => int.TryParse(x, out number));
        }

        public static int CompareVersionTo(string strVersion1, string strVersion2)
        {
            var version1 = new Version(strVersion1);
            var version2 = new Version(strVersion2);
            return version1.CompareTo(version2);
        }
    }
}
