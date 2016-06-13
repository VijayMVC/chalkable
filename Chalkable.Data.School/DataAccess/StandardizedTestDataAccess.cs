using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class StandardizedTestDataAccess : DataAccessBase<StandardizedTest, int>
    {
        public StandardizedTestDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IList<StandardizedTestDetails> GetListOfStandardizedTestDetails(IList<int> ids = null)
        {
            var @params = new Dictionary<string, object>
            {
                ["ids"] = ids ?? new List<int>()
            };
            using (var reader = ExecuteStoredProcedureReader("spGetStandardizedTestDetails", @params))
            {
                var standardizedTest = reader.ReadList<StandardizedTest>();

                reader.NextResult();
                var standardizedTestComponent = reader.ReadList<StandardizedTestComponent>();

                reader.NextResult();
                var standardizedTestScoreType = reader.ReadList<StandardizedTestScoreType>();
  
                return standardizedTest.Select(x => new StandardizedTestDetails
                {
                    Id = x.Id,
                    DisplayName = x.DisplayName,
                    Name = x.Name,
                    DisplayOnTranscript = x.DisplayOnTranscript,
                    StateCode = x.StateCode,
                    SifCode = x.SifCode,
                    NcesCode = x.NcesCode,
                    Description = x.Description,
                    GradeLevelRef = x.GradeLevelRef,
                    Code = x.Code,
                    Components = standardizedTestComponent.Where(stc => x.Id == stc.StandardizedTestRef).ToList(),
                    ScoreTypes = standardizedTestScoreType.Where(stst => x.Id == stst.StandardizedTestRef).ToList()
                }).ToList();
            }
        }
    }
}
