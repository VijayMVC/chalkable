REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.settings.SchoolPersonSettings');
REQUIRE('chlk.models.id.SchoolPersonId');

NAMESPACE('chlk.templates.settings', function () {
    "use strict";


    ASSET('~/assets/jade/activities/settings/school-person-settings-page.jade')();
    /** @class chlk.templates.settings.SchoolPersonSettingsTpl*/
    CLASS(
        [ria.templates.ModelBind(chlk.models.settings.SchoolPersonSettings)],
        'SchoolPersonSettingsTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            Boolean, 'annoucementNotificationsViaSms',
            [ria.templates.ModelPropertyBind],
            Boolean, 'messagesNotificationsViaSms',
            [ria.templates.ModelPropertyBind],
            Boolean, 'notificationsViaSms',
            [ria.templates.ModelPropertyBind],
            Boolean, 'annoucementNotificationsViaEmail',
            [ria.templates.ModelPropertyBind],
            Boolean, 'messagesNotificationsViaEmail',
            [ria.templates.ModelPropertyBind],
            Boolean, 'notificationsViaEmail',
            [ria.templates.ModelPropertyBind],
            chlk.models.id.SchoolPersonId, 'personId'
        ]);
});