REQUIRE('chlk.models.settings.AdminMessaging');
REQUIRE('chlk.templates.ChlkTemplate');

NAMESPACE('chlk.templates.settings', function () {
    "use strict";
    /** @class chlk.templates.settings.AdminMessagingTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/settings/admin-messaging.jade')],
        [ria.templates.ModelBind(chlk.models.settings.MessagingSettingsViewData)],
        'AdminMessagingTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            chlk.models.settings.AdminMessaging, 'messagingSettings',

            [ria.templates.ModelPropertyBind],
            Boolean, 'ableToUpdate',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.apps.Application), 'applications',

            function isAdminPanoramaEnabled() {
                return this.getCurrentUser().getClaims().filter(function(item){
                            return item.hasPermission(chlk.models.people.UserPermissionEnum.MAINTAIN_CHALKABLE_DISTRICT_SETTINGS);
                        }).length > 0;
            }
        ])
});