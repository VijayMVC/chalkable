REQUIRE('chlk.templates.apps.AppWrapperPageTpl');
REQUIRE('chlk.models.settings.AppSettingsViewData');

NAMESPACE('chlk.templates.settings', function () {

    /** @class chlk.templates.settings.AppSettingsPageTpl */
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/settings/app-settings-page.jade')],
        [ria.templates.ModelBind(chlk.models.settings.AppSettingsViewData)],
        'AppSettingsPageTpl', EXTENDS(chlk.templates.apps.AppWrapperPageTpl), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.apps.Application), 'applications',

            function isAdminPanoramaEnabled() {
                return this.getCurrentUser().getClaims().filter(function(item){
                        return item.hasPermission(chlk.models.people.UserPermissionEnum.MAINTAIN_CHALKABLE_DISTRICT_SETTINGS);
                    }).length > 0;
            }
        ]);
});
