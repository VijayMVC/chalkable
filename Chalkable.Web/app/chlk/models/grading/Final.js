REQUIRE('chlk.models.id.FinalGradeId');
REQUIRE('chlk.models.class.Class');
REQUIRE('chlk.models.grading.AnnouncementTypeGrading');

NAMESPACE('chlk.models.grading', function () {
    "use strict";
    /** @class chlk.models.grading.Final*/
    CLASS(
        'Final', [
            chlk.models.id.FinalGradeId, 'id',

            Number, 'state',

            [ria.serialize.SerializeProperty('class')],
            chlk.models.class.Class, 'clazz',

            [ria.serialize.SerializeProperty('gradedstudentcount')],
            Number, 'gradedStudentCount',

            Number, 'participation',

            [ria.serialize.SerializeProperty('droplowestdiscipline')],
            Number, 'dropLowestDiscipline',

            Number, 'attendance',

            [ria.serialize.SerializeProperty('droplowestattendance')],
            Number, 'dropLowestAttendance',

            [ria.serialize.SerializeProperty('gradingstyle')],
            Number, 'gradingStyle',

            [ria.serialize.SerializeProperty('finalgradeanntype')],
            ArrayOf(chlk.models.grading.AnnouncementTypeGrading), 'finalGradeAnnType',


            ArrayOf(Number), 'finalGradeAnnouncementTypeIds',

            ArrayOf(Number), 'percents',

            ArrayOf(Number), 'dropLowest',

            ArrayOf(Number), 'gradingStyleByType',

            Boolean, 'needsTypesForClasses',

            Number, 'nextClassNumber'
        ]);
});
