using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Mapping.ModelMappers
{
    public class DisciplineReferralToClassDisciplineMapper : BaseMapper<ClassDiscipline, DisciplineReferral>
    {
        protected override void InnerMap(ClassDiscipline returnObj, DisciplineReferral sourceObj)
        {
            returnObj.Id = sourceObj.Id;
            returnObj.ClassId = sourceObj.SectionId;
            returnObj.StudentId = sourceObj.StudentId;
            returnObj.Date = sourceObj.Date;
            returnObj.Description = sourceObj.Note;
            if (sourceObj.Infractions != null)
                returnObj.Infractions = sourceObj.Infractions.Select(x => new Data.School.Model.Infraction
                    {
                        InfractionID = x.Id,
                        Name = x.Name
                    }).ToList();
        }
    }
}
