REQUIRE('ria.serialize.SJX');
REQUIRE('ria.serialize.IDeserializable');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.id.ApplicationInstallId');

NAMESPACE('chlk.models.apps', function () {
    "use strict";

    var SJX = ria.serialize.SJX;
    /** @class chlk.models.apps.AppInstallInfo*/
    CLASS(
        UNSAFE, FINAL, 'AppInstallInfo', IMPLEMENTS(ria.serialize.IDeserializable), [
            VOID, function deserialize(raw){
                this.appInstallId = SJX.fromValue(raw.applicationinstallid, chlk.models.id.ApplicationInstallId);
                this.installationOwnerId = SJX.fromValue(raw.installationownerid, chlk.models.id.SchoolPersonId);
                this.owner = SJX.fromValue(raw.isowner, Boolean);
                this.personId = SJX.fromValue(raw.personid, chlk.models.id.SchoolPersonId);
            },
            chlk.models.id.ApplicationInstallId, 'appInstallId',
            chlk.models.id.SchoolPersonId, 'installationOwnerId',
            Boolean, 'owner',
            chlk.models.id.SchoolPersonId, 'personId'
    ])

});
