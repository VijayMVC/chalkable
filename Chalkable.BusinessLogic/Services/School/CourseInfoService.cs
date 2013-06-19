using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{

    public interface ICourseInfoService
    {
        CourseInfo Add(string code, string title, Guid? chalkableDepartmentId = null);
        CourseInfo Edit(Guid courseInfoId, string code, string title, Guid? chalkableDepartmentId = null);
        void Delete(Guid id);
        PaginatedList<CourseInfo> GetCourseInfos(int start, int count);
        CourseInfo GetCourseById(Guid id);
    }

    public class CourseInfoService : SchoolServiceBase, ICourseInfoService
    {
        public CourseInfoService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        //TODO: needs test
        public CourseInfo Add(string code, string title, Guid? chalkableDepartmentId = null)
        {
            if (!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var da = new CourseInfoDataAccess(uow);
                var courseInfo = new CourseInfo
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

        public CourseInfo Edit(Guid courseInfoId, string code, string title, Guid? chalkableDepartmentId = null)
        {
            if (!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();
        
            using (var uow = Update())
            {
                var da = new CourseInfoDataAccess(uow);
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
                var da = new CourseInfoDataAccess(uow);
                da.Delete(id);
                uow.Commit();
            }
        }

        public PaginatedList<CourseInfo> GetCourseInfos(int start, int count)
        {
            using (var uow = Read())
            {
                var da = new CourseInfoDataAccess(uow);
                return  da.GetCourses(start, count);
            }
        }

        public CourseInfo GetCourseById(Guid id)
        {
            using (var uow = Read())
            {
                var da = new CourseInfoDataAccess(uow);
                return da.GetById(id);
            }
        }
    }
}
