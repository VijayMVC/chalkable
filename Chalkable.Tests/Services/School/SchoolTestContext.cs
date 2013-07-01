using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;

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
        public const string CHECKIN_USER = "Checkin";

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
                    CreateUserInfo(res.SecondParentName, CoreRoles.PARENT_ROLE),
                    CreateUserInfo(res.CheckinName, CoreRoles.CHECKIN_ROLE)
                };
            var gradeLevels =  sysSchoolSl.GradeLevelService.CreateDefault();
            CreateUsers(sysSchoolSl, userInfos, gradeLevels[0].Id);
            res.AdminGradeSl = ServiceLocatorFactory.CreateSchoolLocator(res.GetSchoolUser(res.AdminGrade));
            res.AdminEditSl = ServiceLocatorFactory.CreateSchoolLocator(res.GetSchoolUser(res.AdminEdit));
            res.AdminViewSl = ServiceLocatorFactory.CreateSchoolLocator(res.GetSchoolUser(res.AdminView));
            res.FirstTeacherSl = ServiceLocatorFactory.CreateSchoolLocator(res.GetSchoolUser(res.FirstTeacher));
            res.FirstStudentSl = ServiceLocatorFactory.CreateSchoolLocator(res.GetSchoolUser(res.FirstStudent));
            res.FirstParentSl = ServiceLocatorFactory.CreateSchoolLocator(res.GetSchoolUser(res.FirstParent));
            res.SecondTeacherSl = ServiceLocatorFactory.CreateSchoolLocator(res.GetSchoolUser(res.SecondTeahcer));
            res.SecondStudentSl = ServiceLocatorFactory.CreateSchoolLocator(res.GetSchoolUser(res.SecondStudent));
            res.SecondParentSl = ServiceLocatorFactory.CreateSchoolLocator(res.GetSchoolUser(res.SecondParent));
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
        private static Person GetPerson(IServiceLocatorSchool locator, string name, CoreRole role)
        {
            var schoolUser = GetSchoolUser(locator, name, role);
            return locator.PersonService.GetPerson(schoolUser.User.Id);
        }

        public SchoolUser GetSchoolUser(Person person)
        {
            return GetSchoolUser(sysSchoolSl, person.FirstName, CoreRoles.GetById(person.RoleRef));
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
                    Role = role,
                };
        }

        private static void CreateUsers(IServiceLocatorSchool sysSchoolSl, IList<UserIInfoTest> userInfos, Guid? gradeLevelId)
        {
            foreach (var userInfo in userInfos)
            {
                sysSchoolSl.PersonService.Add(userInfo.Login, userInfo.Password, userInfo.FirstName, userInfo.LastName,
                                              userInfo.Role.Name, userInfo.Gender, userInfo.Salutation, userInfo.BirthDate, gradeLevelId);
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
        public string CheckinName
        {
            get { return CHECKIN_USER; }
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

        public IServiceLocatorSchool CheckinSl { get; private set; }

        public Person AdminGrade
        {
            get { return GetPerson(sysSchoolSl, AdminGradeName, CoreRoles.ADMIN_GRADE_ROLE); }
        }

        public Person AdminEdit
        {
            get { return GetPerson(sysSchoolSl, AdminEditName, CoreRoles.ADMIN_EDIT_ROLE); }
        }
        public Person AdminView
        {
            get { return GetPerson(sysSchoolSl, AdminViewName, CoreRoles.ADMIN_VIEW_ROLE); }
        }
        public Person FirstTeacher
        {
            get { return GetPerson(sysSchoolSl, FirstTeacherName, CoreRoles.TEACHER_ROLE); }
        }
        public Person SecondTeahcer
        {
            get { return GetPerson(sysSchoolSl, SecondTeacherName, CoreRoles.TEACHER_ROLE); }
        }
        public Person FirstStudent
        {
            get { return GetPerson(sysSchoolSl, FirstStudentName, CoreRoles.STUDENT_ROLE); }
        }
        public Person SecondStudent
        {
            get { return GetPerson(sysSchoolSl, SecondStudentName, CoreRoles.STUDENT_ROLE); }
        }
        public Person FirstParent
        {
            get { return GetPerson(sysSchoolSl, FirstParentName, CoreRoles.PARENT_ROLE); }
        }
        public Person SecondParent
        {
            get { return GetPerson(sysSchoolSl, SecondParentName, CoreRoles.PARENT_ROLE); }
        }

        public Person Checkin
        {
            get { return GetPerson(sysSchoolSl, CheckinName, CoreRoles.CHECKIN_ROLE); }
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
        Checkin = 512
    }
}
