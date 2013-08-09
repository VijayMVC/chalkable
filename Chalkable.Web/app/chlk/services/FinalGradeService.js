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
                Number, String, String, String, String, Boolean]],
            ria.async.Future, function update(id, participation, attendance, dropLowestAttendance,
                discipline, dropLowestDiscipline, gradingStyle, finalGradeAnnouncementTypeIds,
                percents, dropLowest, gradingStyleByType, needsTypesForClasses) {
                    return this.get('FinalGrade/Update.json', chlk.models.grading.Final, {
                        id: id.valueOf(),
                        participation: participation,
                        attendance: attendance,
                        dropLowestAttendance: dropLowestAttendance,
                        discipline: discipline,
                        dropLowestDiscipline: dropLowestDiscipline,
                        gradingStyle: gradingStyle,
                        finalGradeAnnouncementTypeIds: finalGradeAnnouncementTypeIds,
                        percents: percents,
                        dropLowest: dropLowest,
                        gradingStyleByType: gradingStyleByType,
                        needsTypesForClasses: needsTypesForClasses
                    });
            }
        ])
});