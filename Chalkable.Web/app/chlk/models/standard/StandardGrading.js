REQUIRE('chlk.models.id.StandardId');
REQUIRE('chlk.models.id.GradingPeriodId');
REQUIRE('chlk.models.id.GradeId');
REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.models.standard', function () {
    "use strict";
    /** @class chlk.models.standard.StandardGrading*/
    CLASS(
        'StandardGrading', [
            [ria.serialize.SerializeProperty('gradingperiodid')],
            chlk.models.id.GradingPeriodId, 'gradingPeriodId',

            [ria.serialize.SerializeProperty('standardid')],
            chlk.models.id.StandardId, 'standardId',

            [ria.serialize.SerializeProperty('gradeid')],
            chlk.models.id.GradeId, 'gradeId',

            [ria.serialize.SerializeProperty('studentid')],
            chlk.models.id.SchoolPersonId, 'studentId',

            [ria.serialize.SerializeProperty('classid')],
            chlk.models.id.ClassId, 'classId',

            [ria.serialize.SerializeProperty('gradevalue')],
            String, 'gradeValue',

            String, 'comment',

            [[String, chlk.models.id.StandardId, chlk.models.id.GradingPeriodId, chlk.models.id.SchoolPersonId, chlk.models.id.ClassId, String]],
            function $(gradeValue_, standardId_, gradingPeriodId_, studentId_, classId_, comment_){
                BASE();
                if(gradeValue_)
                    this.gradeValue = gradeValue_;
                if(standardId_)
                    this.standardId = standardId_;
                if(gradingPeriodId_)
                    this.gradingPeriodId = gradingPeriodId_;
                if(studentId_)
                    this.studentId = studentId_;
                if(classId_)
                    this.classId = classId_;
                if(comment_)
                    this.comment = comment_;
            }
        ]);
});
