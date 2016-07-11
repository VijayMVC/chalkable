REQUIRE('chlk.models.settings.AdminPanoramaViewData');
REQUIRE('chlk.templates.ChlkTemplate');

NAMESPACE('chlk.templates.settings', function () {
    "use strict";
    /** @class chlk.templates.settings.AdminPanoramaTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/settings/AdminPanorama.jade')],
        [ria.templates.ModelBind(chlk.models.settings.AdminPanoramaViewData)],
        'AdminPanoramaTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            chlk.models.panorama.AdminPanoramaSettingsViewData, 'panoramaSettings',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.apps.Application), 'applications',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.profile.StandardizedTestViewData), 'standardizedTests'

        ])
});