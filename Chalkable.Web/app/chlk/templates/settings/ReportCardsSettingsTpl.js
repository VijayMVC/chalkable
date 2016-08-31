REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.settings.ReportCardsSettingsViewData');

NAMESPACE('chlk.templates.settings', function () {
    "use strict";
    /** @class chlk.templates.settings.ReportCardsSettingsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/settings/ReportCardsSettings.jade')],
        [ria.templates.ModelBind(chlk.models.settings.ReportCardsSettingsViewData)],
        'ReportCardsSettingsTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.settings.ReportCardsLogo), 'listOfLogo',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.school.School), 'schools',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.apps.Application), 'applications',

            Boolean, 'ableToUpdate',

            Boolean, function isAdminPanoramaEnabled() {
                return this.getCurrentUser().getClaims().filter(function (item) {
                        return item.hasPermission(chlk.models.people.UserPermissionEnum.MAINTAIN_CHALKABLE_DISTRICT_SETTINGS);
                    }).length > 0;
            }

        ])
});