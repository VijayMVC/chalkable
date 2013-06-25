using System;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{

    public interface ICourseService
    {
        Course Add(string code, string title, Guid? chalkableDepartmentId = null);
        Course Edit(Guid courseInfoId, string code, string title, Guid? chalkableDepartmentId = null);
        void Delete(Guid id);
        PaginatedList<Course> GetCourses(int start = 0, int count = int.MaxValue);
        Course GetCourseById(Guid id);
    }

    public class CourseService : SchoolServiceBase, ICourseService
    {
        public CourseService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        //TODO: needs test
        public Course Add(string code, string title, Guid? chalkableDepartmentId = null)
        {
            if (!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var da = new CourseDataAccess(uow);
                var courseInfo = new Course
                    {
                        Id = Guid.NewGuid(),
                        Code = code,
                        Title = title,
                        ChalkableDepartmentRef = chalkableDepartmentId
                    };
                da.Create(courseInfo);
                uow.Commit();
                return courseInfo;
            }
        }

        public Course Edit(Guid courseInfoId, string code, string title, Guid? chalkableDepartmentId = null)
        {
            if (!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();
        
            using (var uow = Update())
            {
                var da = new CourseDataAccess(uow);
                var courseInfo = da.GetById(courseInfoId);
                courseInfo.Title = title;
                courseInfo.Code = code;
                courseInfo.ChalkableDepartmentRef = chalkableDepartmentId;
                da.Update(courseInfo);
                uow.Commit();
                return courseInfo;
            } 
        }

        public void Delete(Guid id)
        {
            if (!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var da = new CourseDataAccess(uow);
                da.Delete(id);
                uow.Commit();
            }
        }

        public PaginatedList<Course> GetCourses(int start, int count)
        {
            using (var uow = Read())
            {
                var da = new CourseDataAccess(uow);
                return  da.GetCourses(start, count);
            }
        }

        public Course GetCourseById(Guid id)
        {
            using (var uow = Read())
            {
                var da = new CourseDataAccess(uow);
                return da.GetById(id);
            }
        }
    }
}
