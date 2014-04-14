using System.Linq;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Mapping.ModelMappers
{
    public class ClassDisciplineToDisciplineReferralMapper : BaseMapper<DisciplineReferral,ClassDiscipline>
    {
        protected override void InnerMap(DisciplineReferral returnObj, ClassDiscipline sourceObj)
        {
            if(sourceObj.Id.HasValue)
                returnObj.Id = sourceObj.Id.Value;
            returnObj.Date = sourceObj.Date;
            returnObj.SectionId = sourceObj.ClassId;
            returnObj.StudentId = sourceObj.StudentId;
            returnObj.Note = sourceObj.Description;
            if (sourceObj.Infractions != null)
                returnObj.Infractions = sourceObj.Infractions.Select(x => new StiConnector.Connectors.Model.Infraction
                    {
                        Id = x.InfractionID, 
                        Name = x.Name
                    }).ToList();
        }
    }
}
