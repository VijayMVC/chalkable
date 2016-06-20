using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.DemoSchool.Common;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Master.Model;


namespace Chalkable.BusinessLogic.Services.DemoSchool.Master
{
    public class DemoMasterSchoolStorage : BaseDemoGuidStorage<Data.Master.Model.School>
    {
        public DemoMasterSchoolStorage()
            : base(x => x.Id)
        {
        }

        public Data.Master.Model.School GetByIdOrNull(Guid id)
        {
            return data.ContainsKey(id) ? data[id] : null;
        }
    }

    public class DemoSchoolService : DemoMasterServiceBase, ISchoolService
    {
        private DemoMasterSchoolStorage MasterSchoolStorage { get; set; }

        public DemoSchoolService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
            MasterSchoolStorage = new DemoMasterSchoolStorage();
        }

        public PaginatedList<Data.Master.Model.School> GetSchools(Guid districtId, int start, int count)
        {
            throw new NotImplementedException();
        }

        public IList<Data.Master.Model.School> GetAll()
        {
            return MasterSchoolStorage.GetAll();
        }

        public void AddMasterSchool(Guid? id)
        {
            if (!id.HasValue)
                throw new ChalkableException("Not valid district id");
            Add(new List<Data.Master.Model.School> { CreateMasterSchool(id.Value) });
        }

        public void Add(Guid districtId, int localId, string name)
        {

            var school = new Data.Master.Model.School
            {
                DistrictRef = districtId,
                Id = Guid.NewGuid(),
                LocalId = localId,
                Name = name
            };
            MasterSchoolStorage.Add(school);
        }

        public static Data.Master.Model.School CreateMasterSchool(Guid id)
        {
            return new Data.Master.Model.School
            {
                DistrictRef = id,
                Id = id,
                LocalId = DemoSchoolConstants.SchoolId,
                Name = "SMITH"
            };
        }

        public void Add(IList<SchoolInfo> schools, Guid districtId)
        {
            MasterSchoolStorage.Add(schools.Select(x => new Data.Master.Model.School
            {
                Name = x.Name,
                LocalId = x.LocalId,
                DistrictRef = districtId,
                Id = Guid.NewGuid()
            }).ToList());
        }

        public void Edit(IList<SchoolInfo> schoolInfos, Guid districtId)
        {
            throw new NotImplementedException();
        }

        public void Delete(IList<int> localIds, Guid districtId)
        {
            throw new NotImplementedException();
        }

        public void UpdateStudyCenterEnabled(Guid? districtId, Guid? schoolId, DateTime? enabledTill)
        {
            throw new NotImplementedException();
        }
        public void UpdateAssessmentEnabled(Guid? districtId, Guid? schoolId, bool enabled)
        {
            throw new NotImplementedException();
        }

        public void UpdateMessagingDisabled(Guid? districtId, Guid? schoolId, bool disbaled)
        {
            throw new NotImplementedException();
        }

        public void UpdateMessagingSettings(Guid? districtId, Guid? schoolId, bool studentMessaging, bool studentToClassOnly,
            bool teacherToStudentMessaging, bool teacherToClassOnly)
        {
            throw new NotImplementedException();
        }

        public MessagingSettings GetDistrictMessaginSettings(Guid districtId)
        {
            throw new NotImplementedException();
        }

        public void Add(IList<Data.Master.Model.School> schools)
        {
            MasterSchoolStorage.Add(schools);
        }
        
        public Data.Master.Model.School GetById(Guid districtRef, int localId)
        {
            return MasterSchoolStorage.GetAll().FirstOrDefault(x => x.DistrictRef == districtRef && x.LocalId == localId);
        }
    }
}