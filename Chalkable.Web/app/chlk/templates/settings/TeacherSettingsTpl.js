REQUIRE('chlk.templates.settings.SchoolPersonSettingsTpl');
REQUIRE('chlk.models.settings.TeacherSettings');

NAMESPACE('chlk.templates.settings', function () {
    "use strict";
    /** @class chlk.templates.settings.TeacherSettingsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/settings/teacher-settings.jade')],
        [ria.templates.ModelBind(chlk.models.settings.TeacherSettings)],
        'TeacherSettingsTpl', EXTENDS(chlk.templates.settings.SchoolPersonSettingsTpl), [])
});