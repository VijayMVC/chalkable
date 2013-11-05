using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;
using Chalkable.Tests.Services.Master;
using Chalkable.Tests.Services.School;

namespace Chalkable.Tests.Services.TestContext
{
    
    public class DistrictTestContext
    {
        public Guid DistrictId { get { return District.Id; } }
        public District District { get; protected set; }
        public IServiceLocatorMaster DistrictLocatorMaster { get; protected set; }

        public IServiceLocatorSchool DistrictLocatorFirstSchool { get; private set; }
        public IServiceLocatorSchool DistrictLocatorSecondSchool { get; private set; }

        public SchoolTestContext FirstSchoolContext { get; protected set; }
        public SchoolTestContext SecondSchoolContext { get; protected set; }

        private const string FIRST_SCHOOL_NAME = "FirstTestSchool";
        private const string SECOND_SCHOOL_NAME = "SecondTestSchool";

        private IServiceLocatorMaster sysMasterLocator;

        public DistrictTestContext(IServiceLocatorMaster masterLocator, District district)
        {
            sysMasterLocator = masterLocator;
            District = district;
            var context = sysMasterLocator.Context;
            DistrictLocatorMaster = new BaseMasterServiceLocatorTest(context);
            FirstSchoolContext = CreateSchoolTextContext(FIRST_SCHOOL_NAME, true, false);
            SecondSchoolContext = CreateSchoolTextContext(SECOND_SCHOOL_NAME, true, false);
            DistrictLocatorFirstSchool = GetDistrictSchoolLocator(FirstSchoolContext);
            DistrictLocatorSecondSchool = GetDistrictSchoolLocator(SecondSchoolContext);
        }

        public static DistrictTestContext Create(IServiceLocatorMaster masterLocator, District district)
        {
            return new DistrictTestContext(masterLocator, district);
        }

        private Data.Master.Model.School CreateSchool(string schoolName, bool isActive, bool isPrivate)
        {
            var schools = sysMasterLocator.SchoolService.GetSchools(DistrictId, 0, int.MaxValue);
            var newSchoolId = schools.Count > 0 ? schools.Max(x => x.LocalId) + 1 : 1;
            var schoolLocator = sysMasterLocator.SchoolServiceLocator(DistrictId, null);
            schoolLocator.SchoolService.Add(new Data.School.Model.School
            {
                Id = newSchoolId,
                IsActive = isActive,
                IsPrivate = isPrivate,
                Name = schoolName
            });
            return sysMasterLocator.SchoolService.GetSchools(DistrictId, 0, int.MaxValue)
                .First(x => x.LocalId == newSchoolId);
        }

        private SchoolTestContext CreateSchoolTextContext(string schoolName, bool isActive, bool isPrivate)
        {
            var school = CreateSchool(schoolName, isActive, isPrivate);
            return SchoolTestContext.Create(ServiceLocatorFactory.CreateMasterSysAdmin().SchoolServiceLocator(DistrictId, school.Id));
        }

        public IServiceLocatorSchool GetDistrictSchoolLocator(SchoolTestContext schoolContext)
        {
            var locator = ServiceLocatorFactory.CreateMasterSysAdmin();
            var context = locator.SchoolServiceLocator(schoolContext.School.Id).Context;
            return new BaseSchoolServiceLocatorTest(new BaseMasterServiceLocatorTest(context));
        }
    }

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
            School = this.sysSchoolSl.ServiceLocatorMaster.SchoolService.GetById(this.sysSchoolSl.Context.SchoolId.Value);
            InitBaseData(this.sysSchoolSl);
        }

        protected virtual void InitBaseData(IServiceLocatorSchool sysSchoolSl)
        {
            AdminGradeSl = CreateUserWithLocator(AdminGradeName, CoreRoles.ADMIN_GRADE_ROLE, null);
            AdminEditSl = CreateUserWithLocator(AdminEditName, CoreRoles.ADMIN_EDIT_ROLE, null);
            AdminViewSl = CreateUserWithLocator(AdminViewName, CoreRoles.ADMIN_VIEW_ROLE, null);
            FirstTeacherSl = CreateUserWithLocator(FirstTeacherName, CoreRoles.TEACHER_ROLE, null);
            FirstStudentSl = CreateUserWithLocator(FirstStudentName, CoreRoles.STUDENT_ROLE, null);
            FirstParentSl = CreateUserWithLocator(FirstParentName, CoreRoles.PARENT_ROLE, null);
            SecondTeacherSl = CreateUserWithLocator(SecondTeacherName, CoreRoles.TEACHER_ROLE, null);
            SecondStudentSl = CreateUserWithLocator(SecondStudentName, CoreRoles.STUDENT_ROLE, null);
            SecondParentSl = CreateUserWithLocator(SecondParentName, CoreRoles.PARENT_ROLE, null);
        }

        public static SchoolTestContext Create(IServiceLocatorSchool sysSchoolSl)
        {
            return new SchoolTestContext(sysSchoolSl);
        }

        private IServiceLocatorSchool CreateUserWithLocator(string name, CoreRole role, Guid? gradeLevelId)
        {
            var userinfo = CreateUserInfo(name, role);
            var schoolAssignments = new List<SchoolAssignmentInfo>
                {
                    new SchoolAssignmentInfo {Role = role.Id, SchoolId = School.LocalId}
                };
            sysSchoolSl.PersonService.Add(userinfo.LocalId, userinfo.Login, userinfo.Password, userinfo.FirstName, userinfo.LastName,
                                              userinfo.Gender, userinfo.Salutation, userinfo.BirthDate, null, null,schoolAssignments);
            return CreateLocatorByUserInfo(userinfo);
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
            var schoolUser = GetSchoolUser(locator, name, role);
            return locator.PersonService.GetPerson(schoolUser.User.LocalId.Value);
        }

        protected UserInfoTest CreateUserInfo(string name, CoreRole role)
        {
            return new UserInfoTest
                {
                    LocalId = ServiceTestBase.GetNewId(sysSchoolSl.PersonService.GetPersons(), x=>x.Id),
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

        public Data.Master.Model.School School { get; protected set; }
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
        Developer = 1024,
        District = 2048
    }
}
