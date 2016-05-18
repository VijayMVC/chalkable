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
            ArrayOf(chlk.models.apps.Application), 'applications'

            //[ria.templates.ModelPropertyBind],
            //Boolean, 'allowedForStudents',
            //
            //[ria.templates.ModelPropertyBind],
            //Boolean, 'allowedForStudentsInTheSameClass',
            //
            //[ria.templates.ModelPropertyBind],
            //Boolean, 'allowedForTeachersToStudents',
            //
            //[ria.templates.ModelPropertyBind],
            //Boolean, 'allowedForTeachersToStudentsInTheSameClass'
        ])
});