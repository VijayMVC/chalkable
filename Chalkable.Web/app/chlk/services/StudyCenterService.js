REQUIRE('chlk.services.BaseService');

REQUIRE('chlk.models.studyCenter.PracticeGradesViewData');
REQUIRE('chlk.models.apps.MiniQuizViewData');

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
            },

            [[chlk.models.id.StandardId]],
            ria.async.Future, function getMiniQuizInfo(standardId) {
                return this.get('StudyCenter/MiniQuizInfo.json', chlk.models.apps.MiniQuizViewData, {
                    standardId : standardId.valueOf()
                });
            }

        ]);
});