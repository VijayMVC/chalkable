REQUIRE('ria.serialize.SJX');
REQUIRE('ria.serialize.IDeserializable');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.id.SchoolId');

NAMESPACE('chlk.models.developer', function () {
    "use strict";

    var SJX = ria.serialize.SJX;
    /** @class chlk.models.developer.DeveloperInfo*/
    CLASS(
        UNSAFE, FINAL,  'DeveloperInfo',IMPLEMENTS(ria.serialize.IDeserializable), [

            VOID, function deserialize(raw){
                this.id = SJX.fromValue(raw.id, chlk.models.id.SchoolPersonId);
                this.displayName = SJX.fromValue(raw.displayname, String);
                this.email = SJX.fromValue(raw.email, String);
                this.firstName = SJX.fromValue(raw.firstname, String);
                this.lastName = SJX.fromValue(raw.lastname, String);
                this.name = SJX.fromValue(raw.name, String);
                this.schoolId = SJX.fromValue(raw.schoolid, chlk.models.id.SchoolId);
                this.webSite = SJX.fromValue(raw.websitelink, String);
                this.payPalAddress = SJX.fromValue(raw.paypallogin, String);
            },

            chlk.models.id.SchoolPersonId, 'id',
            String, 'displayName',
            String, 'email',
            String, 'firstName',
            String, 'lastName',
            String, 'name',
            chlk.models.id.SchoolId, 'schoolId',
            String, 'webSite',
            String, 'payPalAddress'
        ]);
});
