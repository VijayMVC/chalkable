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
    }
}
