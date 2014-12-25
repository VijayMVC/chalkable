REQUIRE('chlk.models.announcement.ClassAnnouncementType');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.grading.AlphaGrade');

NAMESPACE('chlk.models.classes', function () {
    "use strict";

    /** @class chlk.models.classes.ClassForWeekMask*/
    CLASS(
        'ClassForWeekMask', [
            chlk.models.id.ClassId, 'classId',

            ArrayOf(Number), 'mask',

            ArrayOf(chlk.models.announcement.ClassAnnouncementType), 'typesByClass',

            ArrayOf(chlk.models.grading.AlphaGrade), 'alphaGrades',

            ArrayOf(chlk.models.grading.AlphaGrade), 'alphaGradesForStandards'
        ]);
});
