REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.grading.Final');

NAMESPACE('chlk.templates.grading', function () {
    "use strict";
    /** @class chlk.templates.grading.Final*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/setup/teacherSettings.jade')],
        [ria.templates.ModelBind(chlk.models.grading.Final)],
        'Final', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.id.FinalGradeId, 'id',

            [ria.templates.ModelPropertyBind],
            Number, 'state',

            [ria.templates.ModelPropertyBind],
            chlk.models.class.Class, 'clazz',

            [ria.templates.ModelPropertyBind],
            Number, 'gradedStudentCount',

            [ria.templates.ModelPropertyBind],
            Number, 'participation',

            [ria.templates.ModelPropertyBind],
            Number, 'dropLowestDiscipline',

            [ria.templates.ModelPropertyBind],
            Number, 'attendance',

            [ria.templates.ModelPropertyBind],
            Number, 'dropLowestAttendance',

            [ria.templates.ModelPropertyBind],
            Number, 'gradingstyle',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.AnnouncementTypeGrading), 'finalGradeAnnType'
        ]);
});
