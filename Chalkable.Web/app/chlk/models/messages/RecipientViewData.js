REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('ria.serialize.SJX');

NAMESPACE('chlk.models.messages', function () {

    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.messages.RecipientViewData*/
    CLASS(
        'RecipientViewData', IMPLEMENTS(ria.serialize.IDeserializable), [
            chlk.models.id.SchoolPersonId, 'personId',

            String, 'displayName',

            chlk.models.id.ClassId, 'classId',

            String, 'classNumber',

            String, 'className',

            VOID, function deserialize(raw) {
                this.personId = SJX.fromValue(raw.personid, chlk.models.id.SchoolPersonId);
                this.displayName = SJX.fromValue(raw.displayname, String);
                this.classId = SJX.fromValue(raw.classid, chlk.models.id.ClassId);
                this.classNumber = SJX.fromValue(raw.classnumber, String);
                this.className = SJX.fromValue(raw.classname, String);
            }
        ]);
});
