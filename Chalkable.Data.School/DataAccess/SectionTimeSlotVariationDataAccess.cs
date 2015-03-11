using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class SectionTimeSlotVariationDataAccess : BaseSchoolDataAccess<SectionTimeSlotVariation>
    {
        public SectionTimeSlotVariationDataAccess(UnitOfWork unitOfWork, int? localSchoolId) : base(unitOfWork, localSchoolId)
        {
        }

        public void Delete(IList<SectionTimeSlotVariation> sectionTimeSlotVariations)
        {
            SimpleDelete(sectionTimeSlotVariations);
        }
    }
}