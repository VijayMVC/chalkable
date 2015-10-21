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
});
