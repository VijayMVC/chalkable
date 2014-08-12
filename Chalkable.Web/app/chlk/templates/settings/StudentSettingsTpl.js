REQUIRE('chlk.templates.settings.SchoolPersonSettingsTpl');
REQUIRE('chlk.models.settings.SchoolPersonSettings');

NAMESPACE('chlk.templates.settings', function () {
    "use strict";
    /** @class chlk.templates.settings.StudentSettingsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/settings/student-settings.jade')],
        [ria.templates.ModelBind(chlk.models.settings.SchoolPersonSettings)],
        'StudentSettingsTpl', EXTENDS(chlk.templates.settings.SchoolPersonSettingsTpl), [])
});