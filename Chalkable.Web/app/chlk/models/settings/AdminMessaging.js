NAMESPACE('chlk.models.settings', function () {
    var SJX = ria.serialize.SJX;

    "use strict";
    /** @class chlk.models.settings.AdminMessaging*/
    CLASS('AdminMessaging', IMPLEMENTS(ria.serialize.IDeserializable),[
        Boolean, 'allowedForStudents',
        Boolean, 'allowedForStudentsInTheSameClass',
        Boolean, 'allowedForTeachersToStudents',
        Boolean, 'allowedForTeachersToStudentsInTheSameClass',

        VOID, function deserialize(raw) {
            this.allowedForStudents = SJX.fromValue(raw.studentmessagingenabled, Boolean);
            this.allowedForStudentsInTheSameClass = SJX.fromValue(raw.studenttoclassmessagingonly, Boolean);
            this.allowedForTeachersToStudents = SJX.fromValue(raw.teachertostudentmessaginenabled, Boolean);
            this.allowedForTeachersToStudentsInTheSameClass = SJX.fromValue(raw.teachertoclassmessagingonly, Boolean);
        }
    ]);

    /** @class chlk.models.settings.MessagingSettingsViewData*/
    CLASS('MessagingSettingsViewData', [

        chlk.models.settings.AdminMessaging, 'messagingSettings',
        ArrayOf(chlk.models.apps.Application), 'applications',
        Boolean, 'ableToUpdate',

        [[chlk.models.settings.AdminMessaging, ArrayOf(chlk.models.apps.Application), Boolean]],
        function $(messagingSettings_, applications_, ableToUpdate_){
            BASE();
            if(messagingSettings_)
                this.setMessagingSettings(messagingSettings_);
            if(applications_)
                this.setApplications(applications_);
            if(ableToUpdate_)
                this.setAbleToUpdate(ableToUpdate_);
        }
    ]);
});
