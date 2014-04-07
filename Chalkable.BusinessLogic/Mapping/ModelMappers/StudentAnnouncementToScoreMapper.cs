using Chalkable.BusinessLogic.Model;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Mapping.ModelMappers
{
    public class StudentAnnouncementToScoreMapper : BaseMapper<Score, StudentAnnouncement>
    {
        protected override void InnerMap(Score returnObj, StudentAnnouncement sourceObj)
        {
            returnObj.ActivityId = sourceObj.ActivityId;
            returnObj.StudentId = sourceObj.StudentId;
            returnObj.NumericScore = sourceObj.NumericScore;
            returnObj.ScoreValue = sourceObj.ScoreValue;
            returnObj.AlphaGradeId = sourceObj.AlphaGradeId;
            returnObj.AlternateScoreId = sourceObj.AlternateScoreId;
            returnObj.Comment = sourceObj.Comment;
            returnObj.Exempt = sourceObj.Exempt;
            returnObj.Late = sourceObj.Late;
            returnObj.Incomplete = sourceObj.Incomplete;
            returnObj.Absent = sourceObj.Absent;
            returnObj.Dropped = sourceObj.Dropped;
            returnObj.Withdrawn = sourceObj.Withdrawn;
            returnObj.OverMaxScore = sourceObj.Withdrawn;
            returnObj.AbsenceCategory = sourceObj.AbsenceCategory;
        }
    }
}
