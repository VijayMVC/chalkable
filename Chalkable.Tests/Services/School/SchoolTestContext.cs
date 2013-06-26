using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;

namespace Chalkable.Tests.Services.School
{
    public class SchoolTestContext
    {
        public const string SCHOOL_ADMIN_GRADE_USER = "AdminGrade";
        public const string SCHOOL_ADMIN_EDIT_USER = "AdminEdit";
        public const string SCHOOL_ADMIN_VIEW_USER = "AdminView";
        public const string TEACHER_USER = "Teacher";
        public const string STUDENT_USER = "Student";
        public const string PARENT_USER = "Parent";
        public const string DEVELOPER_USER = "Devloper";

        private const string PREFIX1 = "first_";
        private const string PREFIX2 = "second_";

        private const string DEFAULT_MAIL = "@testchalkable.com";
        private const string DEFAULT_PASSWORD = "tester";
        private const string DEFAULT_GENDER = "M";

        private class UserIInfoTest : UserInfo
        {
            public CoreRole Role { get; set; }
        }


        private IServiceLocatorSchool sysSchoolSl;
        private SchoolTestContext(IServiceLocatorSchool sysSchoolSl)
        {
            this.sysSchoolSl = sysSchoolSl;
        }

        public static SchoolTestContext CreateSchoolContext(IServiceLocatorSchool sysSchoolSl)
        {
            var res = new SchoolTestContext(sysSchoolSl);

            var userInfos = new List<UserIInfoTest>
                {
                    CreateUserInfo(res.AdminGradeName, CoreRoles.ADMIN_GRADE_ROLE),
                    CreateUserInfo(res.AdminViewName, CoreRoles.ADMIN_VIEW_ROLE),
                    CreateUserInfo(res.AdminEditName, CoreRoles.ADMIN_EDIT_ROLE),
                    CreateUserInfo(res.FirstTeacherName, CoreRoles.TEACHER_ROLE),
                    CreateUserInfo(res.SecondTeacherName, CoreRoles.TEACHER_ROLE),
                    CreateUserInfo(res.FirstStudentName, CoreRoles.STUDENT_ROLE),
                    CreateUserInfo(res.SecondStudentName, CoreRoles.STUDENT_ROLE),
                    CreateUserInfo(res.FirstParentName, CoreRoles.PARENT_ROLE),
                    CreateUserInfo(res.SecondParentName, CoreRoles.PARENT_ROLE)
                };
            CreateUsers(sysSchoolSl, userInfos);
            res.AdminGradeSl = ServiceLocatorFactory.CreateSchoolLocator(res.AdminGrade);
            res.AdminEditSl = ServiceLocatorFactory.CreateSchoolLocator(res.AdminGrade);
            res.AdminViewSl = ServiceLocatorFactory.CreateSchoolLocator(res.AdminGrade);
            res.FirstTeacherSl = ServiceLocatorFactory.CreateSchoolLocator(res.FirstTeacher);
            res.FirstStudentSl = ServiceLocatorFactory.CreateSchoolLocator(res.FirstStudent);
            res.FirstParentSl = ServiceLocatorFactory.CreateSchoolLocator(res.FirstParent);
            res.SecondTeacherSl = ServiceLocatorFactory.CreateSchoolLocator(res.SecondTeahcer);
            res.SecondStudentSl = ServiceLocatorFactory.CreateSchoolLocator(res.SecondStudent);
            res.SecondParentSl = ServiceLocatorFactory.CreateSchoolLocator(res.SecondParent);
            return res;
        }

        private static string GetUserLogin(string name)
        {
            return name + DEFAULT_MAIL;
        }
        private static SchoolUser GetSchoolUser(IServiceLocatorSchool locator, string name, CoreRole role)
        {
            var user = locator.ServiceLocatorMaster.UserService.GetByLogin(GetUserLogin(name));
            return user.SchoolUsers.First(x => x.Role == role.Id);
        }

        private static UserIInfoTest CreateUserInfo(string name, CoreRole role)
        {
            return new UserIInfoTest
                {
                    Login = GetUserLogin(name),
                    FirstName = name,
                    LastName = name,
                    Gender = DEFAULT_GENDER,
                    Password = DEFAULT_PASSWORD,
                    Role = role
                };
        }

        private static void CreateUsers(IServiceLocatorSchool sysSchoolSl, IList<UserIInfoTest> userInfos)
        {
            foreach (var userInfo in userInfos)
            {
                sysSchoolSl.PersonService.Add(userInfo.Login, userInfo.Password, userInfo.FirstName, userInfo.LastName,
                                              userInfo.Role.Name, userInfo.Gender, userInfo.Salutation, userInfo.BirthDate);
            }
        }

        public string AdminGradeName
        {
            get { return SCHOOL_ADMIN_GRADE_USER; }
        }
        public string AdminEditName
        {
            get { return SCHOOL_ADMIN_EDIT_USER; }
        }
        public string AdminViewName
        {
            get { return SCHOOL_ADMIN_VIEW_USER; }
        }
        public string FirstTeacherName
        {
            get { return PREFIX1 + TEACHER_USER; }
        }
        public string SecondTeacherName
        {
            get { return PREFIX2 + TEACHER_USER; }
        }
        public string FirstStudentName
        {
            get { return PREFIX1 + STUDENT_USER; }
        }
        public string SecondStudentName
        {
            get { return PREFIX2 + STUDENT_USER; }
        }
        public string FirstParentName
        {
            get { return PREFIX1 + PARENT_USER; }
        }
        public string SecondParentName
        {
            get { return PREFIX2 + PARENT_USER; }
        }

        public IServiceLocatorSchool AdminGradeSl { get; private set; }

        public IServiceLocatorSchool AdminEditSl { get; private set; }

        public IServiceLocatorSchool AdminViewSl { get; private set; }

        public IServiceLocatorSchool FirstTeacherSl { get; private set; }

        public IServiceLocatorSchool FirstStudentSl { get; private set; }

        public IServiceLocatorSchool SecondTeacherSl { get; private set; }

        public IServiceLocatorSchool SecondStudentSl { get; private set; }

        public IServiceLocatorSchool FirstParentSl { get; private set; }

        public IServiceLocatorSchool SecondParentSl { get; private set; }

        public SchoolUser AdminGrade
        {
            get { return GetSchoolUser(sysSchoolSl, AdminGradeName, CoreRoles.ADMIN_GRADE_ROLE); }
        }

        public SchoolUser AdminEdit
        {
            get { return GetSchoolUser(sysSchoolSl, AdminEditName, CoreRoles.ADMIN_EDIT_ROLE); }
        }
        public SchoolUser AdminView
        {
            get { return GetSchoolUser(sysSchoolSl, AdminViewName, CoreRoles.ADMIN_VIEW_ROLE); }
        }
        public SchoolUser FirstTeacher
        {
            get { return GetSchoolUser(sysSchoolSl, FirstTeacherName, CoreRoles.TEACHER_ROLE); }
        }
        public SchoolUser SecondTeahcer
        {
            get { return GetSchoolUser(sysSchoolSl, SecondTeacherName, CoreRoles.TEACHER_ROLE); }
        }
        public SchoolUser FirstStudent
        {
            get { return GetSchoolUser(sysSchoolSl, FirstStudentName, CoreRoles.STUDENT_ROLE); }
        }
        public SchoolUser SecondStudent
        {
            get { return GetSchoolUser(sysSchoolSl, SecondStudentName, CoreRoles.STUDENT_ROLE); }
        }
        public SchoolUser FirstParent
        {
            get { return GetSchoolUser(sysSchoolSl, FirstParentName, CoreRoles.PARENT_ROLE); }
        }
        public SchoolUser SecondParent
        {
            get { return GetSchoolUser(sysSchoolSl, SecondParentName, CoreRoles.PARENT_ROLE); }
        }

    }


    [Flags]
    public enum SchoolContextRoles
    {
        AdminGrade = 1,
        AdminEditor = 2,
        AdminViewer = 4,
        FirstTeacher = 8,
        FirstStudent = 16,
        FirstParent = 32,
        SecondTeacher = 64,
        SecondStudent = 128,
        SecondParent = 256,
    }
}
