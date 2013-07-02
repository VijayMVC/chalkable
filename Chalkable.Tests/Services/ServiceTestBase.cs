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
            var fields = type.GetProperties().Where(x => !x.PropertyType.IsClass);
            foreach (var propertyInfo in fields)
            {
                Assert.AreEqual(propertyInfo.GetValue(obj1), propertyInfo.GetValue(obj2));
            }
        }
    }
}
