﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Services.School;
using NUnit.Framework;

namespace Chalkable.Tests.Services.School
{
    public partial class BaseSchoolServiceTest
    {

        public static void AssertForDeny(Action<IServiceLocatorSchool> action, SchoolTestContext context,
                                         SchoolContextRoles forRoles)
        {
            if ((forRoles & SchoolContextRoles.AdminGrade) == SchoolContextRoles.AdminGrade)
                AssertException<Exception>(() => action(context.AdminGradeSl));
            if ((forRoles & SchoolContextRoles.AdminEditor) == SchoolContextRoles.AdminEditor)
                AssertException<Exception>(() => action(context.AdminEditSl));
            if ((forRoles & SchoolContextRoles.AdminViewer) == SchoolContextRoles.AdminViewer)
                AssertException<Exception>(() => action(context.AdminViewSl));
            if ((forRoles & SchoolContextRoles.FirstTeacher) == SchoolContextRoles.FirstTeacher)
                AssertException<Exception>(() => action(context.FirstTeacherSl));
            if ((forRoles & SchoolContextRoles.FirstStudent) == SchoolContextRoles.FirstStudent)
                AssertException<Exception>(() => action(context.FirstStudentSl));
            if ((forRoles & SchoolContextRoles.FirstParent) == SchoolContextRoles.FirstParent)
                AssertException<Exception>(() => action(context.FirstParentSl));
            if ((forRoles & SchoolContextRoles.SecondTeacher) == SchoolContextRoles.SecondTeacher)
                AssertException<Exception>(() => action(context.SecondTeacherSl));
            if ((forRoles & SchoolContextRoles.SecondStudent) == SchoolContextRoles.SecondStudent)
                AssertException<Exception>(() => action(context.SecondStudentSl));
            if ((forRoles & SchoolContextRoles.SecondParent) == SchoolContextRoles.SecondParent)
                AssertException<Exception>(() => action(context.SecondParentSl));
        }


        public static void AssertAreEqual<T>(T obj1, T obj2) where T : new()
        {
            var type = typeof (T);
            var fields = type.GetProperties();
            foreach (var propertyInfo in fields)
            {
                Assert.AreEqual(propertyInfo.GetValue(obj1), propertyInfo.GetValue(obj2));
            }
        }

    }
}
