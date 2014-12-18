REQUIRE('chlk.templates.settings.SchoolPersonSettingsTpl');
REQUIRE('chlk.models.settings.SchoolPersonSettings');

NAMESPACE('chlk.templates.settings', function () {
    "use strict";
    /** @class chlk.templates.settings.AdminSettingsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/settings/admin-settings.jade')],
        [ria.templates.ModelBind(chlk.models.settings.SchoolPersonSettings)],
        'AdminSettingsTpl', EXTENDS(chlk.templates.settings.SchoolPersonSettingsTpl), [])
});