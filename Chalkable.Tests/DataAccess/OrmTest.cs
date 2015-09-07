﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.Master.Model;
using Chalkable.Data.Master.Model.Chlk;
using Chalkable.Data.School.DataAccess;
using NUnit.Framework;

namespace Chalkable.Tests.DataAccess
{
    public class OrmTest : TestBase
    {
        [Test]
        public void TestQueryBuilding()
        {
            long start = DateTime.Now.Ticks;
            var fields = Orm.Fields<User>();
            Debug.WriteLine(fields.JoinString(","));
            var u = new User();
            u.Id = Guid.NewGuid();
            u.Password = "some pwd";
            u.Login = "login";
            u.IsSysAdmin = true;
            u.IsDeveloper = false;
            var q = Orm.SimpleInsert(u, false);
            Debug.WriteLine(q.Sql);
            Debug.WriteLine(q.Parameters.Select(x=>x.Key + "=" + x.Value).JoinString("\n"));

            q = Orm.SimpleUpdate(u);
            Debug.WriteLine(q.Sql);
            Debug.WriteLine(q.Parameters.Select(x => x.Key + "=" + x.Value).JoinString("\n"));

            q = Orm.SimpleDelete(u);
            Debug.WriteLine(q.Sql);
            Debug.WriteLine(q.Parameters.Select(x => x.Key + "=" + x.Value).JoinString("\n"));

            Debug.WriteLine("Duration: " + (DateTime.Now.Ticks - start) / 10000);
        }

        private class SomeClass
        {
            public static readonly string ID_FIELD = F<SomeClass, int>(num => num.Id);
            public int Id { get; set; }
            public static readonly string NAME_FIELD = F<SomeClass, string>(num => num.Name);
            public string Name { get; set; }
        }

        [Test]
        public void ExpressionTreeTest()
        {
            Debug.WriteLine(DateTime.Now.Ticks);
            for (int i = 0; i < 10000; i++)
            {
                var s = F<SomeClass, int>(num => num.Id);
            }
            Debug.WriteLine(DateTime.Now.Ticks);
        }

        private static string F<T1, T2>(Expression<Func<T1, T2>> f)
        {
            var body = (MemberExpression)f.Body;
            return body.Member.Name;
        }

        [Test]
        public void CollectionTest()
        {
            var cc = new CustomCollection(){{5, 6}, {new object()}};
        }
    }

    public class CustomCollection : ICollection<object>
    {
        public IEnumerator<object> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(object item)
        {
            throw new NotImplementedException();
        }

        public void Add(int a, int b)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(object item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(object[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(object item)
        {
            throw new NotImplementedException();
        }

        public int Count { get; private set; }
        public bool IsReadOnly { get; private set; }
    }

    
}
