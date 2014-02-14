using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Mapping.ModelMappers
{
    public class StudentAnnouncementToScoreMapper : BaseMapper<Score, StudentAnnouncement>
    {
        protected override void InnerMap(Score returnObj, StudentAnnouncement sourceObj)
        {
            returnObj.StudentId = sourceObj.PersonRef;
            returnObj.NumericScore = sourceObj.GradeValue;
            returnObj.ScoreValue = sourceObj.StiScoreValue;
            returnObj.AlphaGradeId = sourceObj.AlphaGradeId;
            returnObj.AlternateScoreId = sourceObj.AlternateScoreId;
            returnObj.Comment = sourceObj.Comment;
            returnObj.Exempt = sourceObj.Exempt;
            returnObj.Late = sourceObj.Late;
            returnObj.Incomplete = sourceObj.Incomplete;
            returnObj.Absent = sourceObj.Absent;
            returnObj.Dropped = sourceObj.Dropped;
        }
    }
}
