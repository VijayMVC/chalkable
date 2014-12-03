REQUIRE('chlk.templates.setup.TeacherSettings');
REQUIRE('chlk.models.setup.TeacherSettings');

NAMESPACE('chlk.templates.grading', function () {
    "use strict";
    /** @class chlk.templates.grading.TeacherSettings*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/teacherSettings.jade')],
        [ria.templates.ModelBind(chlk.models.setup.TeacherSettings)],
        'TeacherSettings', EXTENDS(chlk.templates.setup.TeacherSettings), []);
});
