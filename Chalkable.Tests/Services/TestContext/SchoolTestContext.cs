﻿using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;
using Chalkable.Tests.Services.Master;
using Chalkable.Tests.Services.School;

namespace Chalkable.Tests.Services.TestContext
{
    public class SchoolTestContext
    {
        public const string SCHOOL_ADMIN_GRADE_USER = "AdminGrade";
        public const string SCHOOL_ADMIN_EDIT_USER = "AdminEdit";
        public const string SCHOOL_ADMIN_VIEW_USER = "AdminView";
        public const string TEACHER_USER = "Teacher";
        public const string STUDENT_USER = "Student";
        public const string PARENT_USER = "Parent";
        public const string CHECKIN_USER = "Checkin";

        private const string PREFIX1 = "first_";
        private const string PREFIX2 = "second_";

        private const string DEFAULT_MAIL = "@testchalkable.com";
        private const string DEFAULT_PASSWORD = "tester";
        private const string DEFAULT_GENDER = "M";

        protected class UserInfoTest : UserInfo
        {
            public CoreRole Role { get; set; }
        }

        private IServiceLocatorSchool sysSchoolSl;

        protected SchoolTestContext(IServiceLocatorSchool sysSchoolSl)
        {
            this.sysSchoolSl = sysSchoolSl;
            InitBaseData(this.sysSchoolSl);
        }

        protected virtual void InitBaseData(IServiceLocatorSchool sysSchoolSl)
        {
            var gradeLevels = sysSchoolSl.GradeLevelService.CreateDefault();
            AdminGradeSl = CreateUserWithLocator(AdminGradeName, CoreRoles.ADMIN_GRADE_ROLE, null);
            AdminEditSl = CreateUserWithLocator(AdminEditName, CoreRoles.ADMIN_EDIT_ROLE, null);
            AdminViewSl = CreateUserWithLocator(AdminViewName, CoreRoles.ADMIN_VIEW_ROLE, null);
            FirstTeacherSl = CreateUserWithLocator(FirstTeacherName, CoreRoles.TEACHER_ROLE, null);
            //FirstStudentSl = CreateUserWithLocator(FirstStudentName, CoreRoles.STUDENT_ROLE, gradeLevels[0].Id);
            FirstParentSl = CreateUserWithLocator(FirstParentName, CoreRoles.PARENT_ROLE, null);
            SecondTeacherSl = CreateUserWithLocator(SecondTeacherName, CoreRoles.TEACHER_ROLE, null);
            //SecondStudentSl = CreateUserWithLocator(SecondStudentName, CoreRoles.STUDENT_ROLE, gradeLevels[0].Id);
            SecondParentSl = CreateUserWithLocator(SecondParentName, CoreRoles.PARENT_ROLE, null);
        }

        public static SchoolTestContext Create(IServiceLocatorSchool sysSchoolSl)
        {
            return new SchoolTestContext(sysSchoolSl);
        }

        private IServiceLocatorSchool CreateUserWithLocator(string name, CoreRole role, Guid? gradeLevelId)
        {
            throw new NotImplementedException();
            //var userinfo = CreateUserInfo(name, role);
            //sysSchoolSl.PersonService.Add(userinfo.Login, userinfo.Password, userinfo.FirstName, userinfo.LastName,
            //                                  userinfo.Role.Name, userinfo.Gender, userinfo.Salutation, userinfo.BirthDate, gradeLevelId);
            //return CreateLocatorByUserInfo(userinfo);
        }

        private IServiceLocatorSchool CreateLocatorByUserInfo(UserInfoTest userInfo)
        {
            var context = sysSchoolSl.ServiceLocatorMaster.UserService.Login(userInfo.Login, userInfo.Password);
            var masterLocator = new BaseMasterServiceLocatorTest(context);
            return new BaseSchoolServiceLocatorTest(masterLocator);
        }

        private static string GetUserLogin(string name, Guid schoolId)
        {
            return name + "_" + schoolId + "_" + DEFAULT_MAIL;
        }
        private static SchoolUser GetSchoolUser(IServiceLocatorSchool locator, string name, CoreRole role)
        {
            var user = locator.ServiceLocatorMaster.UserService.GetByLogin(GetUserLogin(name, locator.Context.SchoolId.Value));
            return user.SchoolUsers.First(x => x.Role == role.Id);
        }
        private static Person GetPerson(IServiceLocatorSchool locator, string name, CoreRole role)
        {
            throw new NotImplementedException();
            //var schoolUser = GetSchoolUser(locator, name, role);
            //return locator.PersonService.GetPerson(schoolUser.User.Id);
        }

        protected UserInfoTest CreateUserInfo(string name, CoreRole role)
        {
            return new UserInfoTest
                {
                    Login = GetUserLogin(name, sysSchoolSl.Context.SchoolId.Value),
                    FirstName = name,
                    LastName = name,
                    Gender = DEFAULT_GENDER,
                    Password = DEFAULT_PASSWORD,
                    Role = role,
                };
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

        public DateTime NowTime
        {
            get { return AdminGradeSl.Context.NowSchoolTime; }
        }
        public DateTime NowDate
        {
            get { return NowTime.Date; }
        }
        public int NowMinutes
        {
            get { return (int) (NowTime - NowDate).TotalMinutes; }
        }

        public Data.Master.Model.School School
        {
            get { return sysSchoolSl.ServiceLocatorMaster.SchoolService.GetById(sysSchoolSl.Context.SchoolId.Value); }
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
        Checkin = 512,
        Developer = 1024
    }
}
