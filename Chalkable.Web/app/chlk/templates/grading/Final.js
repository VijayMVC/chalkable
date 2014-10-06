REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.grading.Final');

NAMESPACE('chlk.templates.grading', function () {
    "use strict";
    /** @class chlk.templates.grading.Final*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/annTypesSettings.jade')],
        [ria.templates.ModelBind(chlk.models.grading.Final)],
        'Final', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.id.FinalGradeId, 'id',

            [ria.templates.ModelPropertyBind],
            Number, 'state',

            [ria.templates.ModelPropertyBind],
            chlk.models.classes.Class, 'clazz',

            [ria.templates.ModelPropertyBind],
            Number, 'gradedStudentCount',

            [ria.templates.ModelPropertyBind],
            Number, 'participation',

            [ria.templates.ModelPropertyBind],
            Number, 'discipline',

            [ria.templates.ModelPropertyBind],
            Number, 'dropLowestDiscipline',

            [ria.templates.ModelPropertyBind],
            Number, 'attendance',

            [ria.templates.ModelPropertyBind],
            Number, 'dropLowestAttendance',

            [ria.templates.ModelPropertyBind],
            Number, 'gradingStyle',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.AnnouncementTypeGrading), 'finalGradeAnnType',

            ArrayOf(chlk.models.grading.AnnouncementTypeFinal), 'finalGradeAnnTypeSend',


            [ria.templates.ModelPropertyBind],
            String, 'finalGradeAnnouncementTypeIds',

            [ria.templates.ModelPropertyBind],
            String, 'percents',

            [ria.templates.ModelPropertyBind],
            Boolean, 'changed',

            [ria.templates.ModelPropertyBind],
            String, 'submitType',

            [ria.templates.ModelPropertyBind],
            String, 'dropLowest',

            [ria.templates.ModelPropertyBind],
            String, 'gradingStyleByType',

            [ria.templates.ModelPropertyBind],
            Boolean, 'needsTypesForClasses',

            [ria.templates.ModelPropertyBind],
            Number, 'nextClassNumber'
        ]);
});
