REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.people.Role');

NAMESPACE('chlk.models.reports', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.reports.UserForReport*/
    CLASS(
        'UserForReport', IMPLEMENTS(ria.serialize.IDeserializable),  [

            String, 'displayName',
            String, 'comment',
            String, 'gender',
            chlk.models.id.SchoolPersonId, 'id',
            chlk.models.people.Role, 'role',

            String, 'pictureUrl',

            VOID, function deserialize(raw){
                this.id = SJX.fromValue(raw.id, chlk.models.id.SchoolPersonId);
                this.displayName = SJX.fromValue(raw.displayname, String);
                this.gender = SJX.fromValue(raw.gender, String);
                this.comment = SJX.fromValue(raw.comment, String);
                this.role = SJX.fromDeserializable(raw.role, chlk.models.people.Role);
            }
        ]);

});
