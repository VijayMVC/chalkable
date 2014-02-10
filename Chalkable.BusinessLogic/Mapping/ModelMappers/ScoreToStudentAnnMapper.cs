using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Mapping.ModelMappers
{
    public class ScoreToStudentAnnMapper : BaseMapper<StudentAnnouncement,Score>
    {
        protected override void InnerMap(StudentAnnouncement returnObj, Score sourceObj)
        {
            returnObj.Comment = sourceObj.Comment;
            returnObj.Dropped = sourceObj.Dropped;
            returnObj.GradeValue = (int?) sourceObj.NumericScore;
            returnObj.PersonRef = sourceObj.StudentId;
            returnObj.StiScoreValue = sourceObj.ScoreValue;
            returnObj.AlphaGradeId = sourceObj.AlphaGradeId;
            returnObj.AlternateScoreId = sourceObj.AlternateScoreId;
        }
    }
}
