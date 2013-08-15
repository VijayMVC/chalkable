REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.settings.TeacherSettings');

NAMESPACE('chlk.templates.settings', function () {

    /** @class chlk.templates.settings.TeacherSettings*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/settings/TeacherSettings.jade')],
        [ria.templates.ModelBind(chlk.models.settings.TeacherSettings)],
        'TeacherSettings', EXTENDS(chlk.templates.JadeTemplate), [
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
            Boolean, 'notificationsViaEmail'
        ])
});