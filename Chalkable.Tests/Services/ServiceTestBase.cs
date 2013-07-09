using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Chalkable.Tests.Services
{
    public class ServiceTestBase : OnDataBaseTest
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

        protected static void AssertAreEqual<T>(T obj1, T obj2) where T : new()
        {
            var type = typeof(T);
            var fields = type.GetProperties().Where(x => !x.PropertyType.IsClass && !x.PropertyType.IsInterface).ToList();
            foreach (var propertyInfo in fields)
            {
                Assert.AreEqual(propertyInfo.GetValue(obj1), propertyInfo.GetValue(obj2));
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
            
            var type = typeof (T);
            var isObject = type.IsClass || type.IsInterface;
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

    }
}
