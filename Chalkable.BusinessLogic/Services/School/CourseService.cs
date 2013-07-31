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
        Course Add(string code, string title, byte[] icon, Guid? chalkableDepartmentId = null, int? sisId = null);
        Course Edit(Guid courseInfoId, string code, string title, byte[] icon, Guid? chalkableDepartmentId = null);
        void Delete(Guid id);
        PaginatedList<Course> GetCourses(int start = 0, int count = int.MaxValue);
        Course GetCourseById(Guid id);
    }

    public class CourseService : SchoolServiceBase, ICourseService
    {
        public CourseService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        private bool DepartmentExists(Guid? chalkableDepartmentId)
        {
            var departmentService = ServiceLocator.ServiceLocatorMaster.ChalkableDepartmentService;
            return !chalkableDepartmentId.HasValue || 
                departmentService.GetChalkableDepartmentById(chalkableDepartmentId.Value) != null;
        }
        private Course Edit(Course course, string code, string title, byte[] icon, Guid? chalkableDepartmentId = null)
        {
            if (!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();

            if (!DepartmentExists(chalkableDepartmentId))
                throw new ChalkableException("There are no department with such id");

            course.Title = title;
            course.Code = code;
            course.ChalkableDepartmentRef = chalkableDepartmentId;
            var masterLocator = ServiceLocator.ServiceLocatorMaster;
            if (icon == null && chalkableDepartmentId.HasValue)
            {
                var department = masterLocator.ChalkableDepartmentService.GetChalkableDepartmentById(chalkableDepartmentId.Value);
                icon = masterLocator.DepartmentIconService.GetPicture(department.Id, null, null);
            }
            masterLocator.CourseIconService.UploadPicture(course.Id, icon);
            return course;
        }


        //TODO: needs test
        public Course Add(string code, string title, byte[] icon, Guid? chalkableDepartmentId = null, int? sisId = null)
        {
            using (var uow = Update())
            {                
                var da = new CourseDataAccess(uow);
                var res = new Course {Id = Guid.NewGuid()};
                res = Edit(res, code, title, icon, chalkableDepartmentId);
                res.SisId = sisId;
                da.Insert(res);
                uow.Commit();
                return res;
            }
        }
        public Course Edit(Guid courseId, string code, string title, byte[] icon, Guid? chalkableDepartmentId = null)
        {
            using (var uow = Update())
            {
                var da = new CourseDataAccess(uow);
                var course = da.GetById(courseId);
                course =  Edit(course, code, title, icon, chalkableDepartmentId);
                da.Update(course);
                uow.Commit();
                return course;
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
                ServiceLocator.ServiceLocatorMaster.CourseIconService.DeletePicture(id);
                uow.Commit();
            }
        }

        public PaginatedList<Course> GetCourses(int start, int count)
        {
            using (var uow = Read())
            {
                var da = new CourseDataAccess(uow);
                return  da.GetPage(start, count);
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
