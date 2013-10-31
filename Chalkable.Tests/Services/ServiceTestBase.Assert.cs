using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Tests.Services.School;
using NUnit.Framework;

namespace Chalkable.Tests.Services
{
    public partial class ServiceTestBase
    {
        protected static void AssertException<TExc>(Action action) where TExc : Exception
        {
            bool wasException = false;
            try
            {
                action();
            }
            catch (Exception ex)
            {
                if (ex is TExc)
                    wasException = true;
            }
            Assert.IsTrue(wasException);
        }

        protected static void AssertAreEqual<T>(T expected, T actual) where T : new()
        {
            var type = typeof(T);
            var fields = type.GetProperties().Where(x => !x.PropertyType.IsClass && !x.PropertyType.IsInterface).ToList();
            foreach (var propertyInfo in fields)
            {
                Assert.AreEqual(propertyInfo.GetValue(expected), propertyInfo.GetValue(actual));
            }
        }

        protected static void AssertAreEqual<T>(IList<T> expected, IList<T> actual) where T : new()
        {
            if (expected == null && actual == null)
                return;
            if (expected == null)
                throw new Exception("Expected list is null but actual isn't");
            if (actual == null)
                throw new Exception("Actual list is null but expected isn't");
            if (expected.Count != actual.Count)
                throw new Exception(string.Format("Different size of lists. expected {0} actual {1}", expected.Count, actual.Count));

            var type = typeof(T);
            var isObject = (type.IsClass || type.IsInterface) && typeof(Guid) != type && typeof(Guid?) != type;
            for (int i = 0; i < expected.Count; i++)
            {
                if (isObject)
                {
                    AssertAreEqual(expected[i], actual[i]);
                }
                else
                {
                    Assert.AreEqual(expected[i], actual[i]);
                }
            }
        }

        public static int GetNewId<TModel>(IList<TModel> models, Func<TModel, int> action)
        {
            return models.Count > 0 ? models.Max(action) + 1 : 1;
        }

    }
}
