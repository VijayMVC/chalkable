REQUIRE('chlk.models.id.StandardId');
REQUIRE('chlk.models.id.GradingPeriodId');
REQUIRE('chlk.models.id.GradeId');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('ria.serialize.SJX');

NAMESPACE('chlk.models.standard', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.standard.StandardGrading*/
    CLASS(
        FINAL, UNSAFE, 'StandardGrading', IMPLEMENTS(ria.serialize.IDeserializable), [
            chlk.models.id.GradingPeriodId, 'gradingPeriodId',
            chlk.models.id.StandardId, 'standardId',
            chlk.models.id.GradeId, 'gradeId',
            chlk.models.id.SchoolPersonId, 'studentId',
            chlk.models.id.ClassId, 'classId',
            String, 'gradeValue',
            String, 'comment',

            VOID, function deserialize(raw){
                this.gradingPeriodId = SJX.fromValue(raw.gradingperiodid, chlk.models.id.GradingPeriodId);
                this.standardId = SJX.fromValue(raw.standardid, chlk.models.id.StandardId);
                this.gradeId = SJX.fromValue(raw.gradeid, chlk.models.id.GradeId);
                this.studentId = SJX.fromValue(raw.studentid, chlk.models.id.SchoolPersonId);
                this.classId = SJX.fromValue(raw.classid, chlk.models.id.ClassId);
                this.gradeValue = SJX.fromValue(raw.gradevalue, String);
                this.comment = SJX.fromValue(raw.comment, String);
            },

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
