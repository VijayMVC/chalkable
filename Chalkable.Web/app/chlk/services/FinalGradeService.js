REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.grading.Final');
REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.FinalGradeService */
    CLASS(
        'FinalGradeService', EXTENDS(chlk.services.BaseService), [
            [[chlk.models.id.ClassId, Boolean]],
            ria.async.Future, function getFinalGrades(classId, needBuildItems_) {
                return this.get('FinalGrade/Get.json', chlk.models.grading.Final, {
                    classId: classId.valueOf(),
                    needBuildItems: needBuildItems_
                });
            },

            [[chlk.models.id.FinalGradeId, Number, Number, Boolean, Number, Boolean,
                Number, Array, Boolean]],
            ria.async.Future, function update(id, participation, attendance, dropLowestAttendance,
                discipline, dropLowestDiscipline, gradingStyle, finalGradeAnnouncementTypes, needsTypesForClasses) {
                    return this.post('FinalGrade/Update.json', chlk.models.grading.Final, {
                        finalGradeId: id.valueOf(),
                        participationPercent: participation,
                        attendance: attendance,
                        dropLowestAttendance: dropLowestAttendance,
                        discipline: discipline,
                        dropLowestDiscipline: dropLowestDiscipline,
                        gradingStyle: gradingStyle,

                        finalGradeAnnouncementType: finalGradeAnnouncementTypes,
                        needsTypesForClasses: needsTypesForClasses
                    }
                );
            }
        ])
});