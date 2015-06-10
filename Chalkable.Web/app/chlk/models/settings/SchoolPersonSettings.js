

NAMESPACE('chlk.models.settings', function () {
    "use strict";
    /** @class chlk.models.settings.SchoolPersonSettings*/
    CLASS(
        'SchoolPersonSettings', [
            Boolean, 'annoucementNotificationsViaSms',
            Boolean, 'messagesNotificationsViaSms',
            Boolean, 'notificationsViaSms',
            Boolean, 'annoucementNotificationsViaEmail',
            Boolean, 'messagesNotificationsViaEmail',
            Boolean, 'notificationsViaEmail',
            chlk.models.id.SchoolPersonId, 'personId',

            [[chlk.models.id.SchoolPersonId]],
            function $(personId_){
                BASE();
                if(personId_)
                    this.setPersonId(personId_);
            }
        ]);
});
