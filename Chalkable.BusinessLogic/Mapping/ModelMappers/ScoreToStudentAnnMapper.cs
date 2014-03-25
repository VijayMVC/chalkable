using Chalkable.BusinessLogic.Model;
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
            returnObj.NumericScore = (int?) sourceObj.NumericScore;
            returnObj.StudentId = sourceObj.StudentId;
            returnObj.ActivityId = sourceObj.ActivityId;
            returnObj.ScoreValue = sourceObj.ScoreValue;
            returnObj.AlphaGradeId = sourceObj.AlphaGradeId;
            returnObj.AlternateScoreId = sourceObj.AlternateScoreId;
            returnObj.Exempt = sourceObj.Exempt;
            returnObj.Absent = sourceObj.Absent;
            returnObj.Late = sourceObj.Late;
            returnObj.Incomplete = sourceObj.Incomplete;
            returnObj.Withdrawn = sourceObj.Withdrawn;
            returnObj.OverMaxScore = sourceObj.OverMaxScore;
        }
    }
}
