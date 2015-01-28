REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.StudyCenterService */
    CLASS(
        'StudyCenterService', EXTENDS(chlk.services.BaseService), [
            [[chlk.models.id.SchoolPersonId, chlk.models.id.ClassId, chlk.models.id.StandardId]],
            ria.async.Future, function getPracticeGrades(studentId, classId, standardId_) {
                return this.get('StudyCenter/PracticeGrades.json', chlk.models.studyCenter.PracticeGradesViewData, {
                    studentId : studentId.valueOf(),
                    classId : classId.valueOf(),
                    standardId : standardId_ && standardId_.valueOf()
                });
            }
        ]),

        [[chlk.models.id.SchoolPersonId, chlk.models.id.StandardId, chlk.models.id.AppId, String]],
            ria.async.Future, function setPracticeGrade(studentId, standardId, applicationId, score) {
                return this.get('StudyCenter/SetPracticeGrade.json', Boolean, {
                    studentId : studentId.valueOf(),
                    applicationId : applicationId.valueOf(),
                    standardId : standardId.valueOf(),
                    score: score
                });
            }
});