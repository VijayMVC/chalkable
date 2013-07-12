using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class FinalGradeAnnouncementTypeDataAccess : DataAccessBase<FinalGradeAnnouncementType>
    {
        public FinalGradeAnnouncementTypeDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        private Dictionary<string, object> BuildConditions(FinalGradeAnnouncementTypeQuery query)
        {
            var res = new Dictionary<string, object>();
            if(query.Id.HasValue)
                res.Add("Id", query.Id);
            if (query.FinalGradeId.HasValue)
                res.Add("FinalGradeRef", query.FinalGradeId);
            if (query.AnnouncementTypeId.HasValue)
                res.Add("AnnouncementTypeRef", query.AnnouncementTypeId);
            return res;
        }
        public IList<FinalGradeAnnouncementType> GetList(FinalGradeAnnouncementTypeQuery query)
        {
            return SelectMany<FinalGradeAnnouncementType>(BuildConditions(query));
        }
        public FinalGradeAnnouncementType GetOne(FinalGradeAnnouncementTypeQuery query)
        {
            return SelectOne<FinalGradeAnnouncementType>(BuildConditions(query));
        }
        public FinalGradeAnnouncementType GetOneOrNull(FinalGradeAnnouncementTypeQuery query)
        {
            return SelectOneOrNull<FinalGradeAnnouncementType>(BuildConditions(query));
        }
    }

    public class  FinalGradeAnnouncementTypeQuery
    {
        public Guid? Id { get; set; }
        public Guid? FinalGradeId { get; set; }
        public Guid? AnnouncementTypeId { get; set; }
    }
}
