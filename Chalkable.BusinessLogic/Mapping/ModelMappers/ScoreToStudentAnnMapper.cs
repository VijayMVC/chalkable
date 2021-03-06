﻿using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Mapping.ModelMappers
{
    public class ScoreToStudentAnnMapper : BaseMapper<StudentAnnouncement,Score>
    {
        protected override void InnerMap(StudentAnnouncement returnObj, Score sourceObj)
        {
            returnObj.Comment = sourceObj.Comment;
            returnObj.ScoreDropped = sourceObj.Dropped;
            returnObj.AnnouncementDropped = sourceObj.ActivityDropped;
            returnObj.CategoryDropped = sourceObj.CategoryDropped;
            returnObj.AverageDropped = sourceObj.AverageDropped;
            returnObj.NumericScore = sourceObj.NumericScore;
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
            returnObj.AbsenceCategory = sourceObj.AbsenceCategory;
            returnObj.AnnouncementTitle = sourceObj.ActivityName;
        }
    }
}
