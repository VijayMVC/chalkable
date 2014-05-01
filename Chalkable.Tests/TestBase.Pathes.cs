using System.IO;
using System.Reflection;

namespace Chalkable.Tests
{
    public partial class TestBase
    {
        private static string root;

        public static string Root
        {
            get
            {
                if (root == null)
                {
                    root = Assembly.GetAssembly(typeof(TestBase)).CodeBase;
                    root = root.Replace("file:///", "");
                    root = root.Substring(0, root.LastIndexOf('/') + 1);
                    root = root.Replace('/', '\\');
                }
                return root;
            }
        }
        
        protected string SQLRoot
        {
            get
            {
                var res = Directory.GetParent(Root).Parent.Parent.Parent.FullName;
                res = Path.Combine(res, "SQL");
                return res;
            }
        }
        
        public static string DefaulImage1Path
        {
            get 
            { 
                var res = Directory.GetParent(Root).Parent.Parent.FullName;
                return Path.Combine(res, "Default image\\.no-avatar.png");
            }
        }
        public static string DefaulImage2Path
        {
            get
            {
                var res = Directory.GetParent(Root).Parent.Parent.FullName;
                return Path.Combine(res, "Default image\\.no-avatar-female.png");
            }
        }
    }
}
