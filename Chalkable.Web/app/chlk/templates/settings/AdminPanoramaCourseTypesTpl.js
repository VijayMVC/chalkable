REQUIRE('chlk.models.settings.AdminPanoramaViewData');
REQUIRE('chlk.templates.ChlkTemplate');

NAMESPACE('chlk.templates.settings', function () {
    "use strict";
    /** @class chlk.templates.settings.AdminPanoramaCourseTypesTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/settings/AdminPanoramaCourseTypes.jade')],
        [ria.templates.ModelBind(chlk.models.settings.AdminPanoramaViewData)],
        'AdminPanoramaCourseTypesTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            chlk.models.panorama.AdminPanoramaSettingsViewData, 'panoramaSettings',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.profile.StandardizedTestViewData), 'standardizedTests'

        ])
});