NAMESPACE('chlk.models.settings', function () {
    "use strict";
    /** @class chlk.models.settings.TeacherSettings*/
    CLASS(
        'TeacherSettings', [
            Boolean, 'annoucementNotificationsViaSms',
            Boolean, 'messagesNotificationsViaSms',
            Boolean, 'notificationsViaSms',
            Boolean, 'annoucementNotificationsViaEmail',
            Boolean, 'messagesNotificationsViaEmail',
            Boolean, 'notificationsViaEmail'
        ]);
});
