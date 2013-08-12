REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.setup.TeacherSettings');

NAMESPACE('chlk.templates.setup', function () {
    "use strict";
    /** @class chlk.templates.setup.TeacherSettings*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/setup/teacherSettings.jade')],
        [ria.templates.ModelBind(chlk.models.setup.TeacherSettings)],
        'TeacherSettings', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.class.ClassesForTopBar, 'topData',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.calendar.TeacherSettingsCalendarDay), 'calendarInfo',

            [ria.templates.ModelPropertyBind],
            chlk.models.grading.Final, 'gradingInfo',

            [ria.templates.ModelPropertyBind],
            Number, 'percentsSum'
        ]);
});
