REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.id.StandardId');
REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.studyCenter.PracticeGradeViewData');

NAMESPACE('chlk.models.studyCenter', function(){
    "use strict";

    var SJX = ria.serialize.SJX;

    /**@class chlk.models.studyCenter.PracticeGradesViewData*/

    CLASS(
        UNSAFE, 'PracticeGradesViewData', EXTENDS(chlk.models.common.PageWithClasses), IMPLEMENTS(ria.serialize.IDeserializable), [

            VOID, function deserialize(raw) {
                this.practiceGrades = SJX.fromArrayOfDeserializables(raw.practicegrades, chlk.models.studyCenter.PracticeGradeViewData);
                this.standards = SJX.fromArrayOfDeserializables(raw.standrds, chlk.models.standard.Standard);
                this.classId = SJX.fromValue(raw.classId, chlk.models.id.ClassId);
                this.studentId = SJX.fromValue(raw.studentId, chlk.models.id.SchoolPersonId);
                this.standardId = SJX.fromValue(raw.standardId, chlk.models.id.StandardId);
                this.startPractice = SJX.fromValue(raw.startPractice, Boolean);
            },

            ArrayOf(chlk.models.standard.Standard), 'standards',

            ArrayOf(chlk.models.studyCenter.PracticeGradeViewData), 'practiceGrades',

            chlk.models.id.ClassId, 'classId',

            chlk.models.id.SchoolPersonId, 'studentId',

            chlk.models.id.StandardId, 'standardId',

            Boolean, 'startPractice',

            [[chlk.models.classes.ClassesForTopBar]],
            function $(topData_){
                BASE(topData_);
            }
    ]);
});