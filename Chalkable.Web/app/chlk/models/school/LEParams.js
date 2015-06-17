REQUIRE('ria.serialize.SJX');
REQUIRE('ria.serialize.IDeserializable');

NAMESPACE('chlk.models.school', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.school.LEParams*/
    CLASS(
        UNSAFE, FINAL,  'LEParams', IMPLEMENTS(ria.serialize.IDeserializable), [
        VOID, function deserialize(raw) {
            this.leLinkStatus = SJX.fromValue(raw.lelinkstatus, Boolean);
            this.leEnabled = SJX.fromValue(raw.leenabled, Boolean);
            this.leSyncComplete = SJX.fromValue(raw.lesynccomplete, Boolean);
            this.leBaseUrl = SJX.fromValue(raw.lebaseurl, String);
            this.enabledForUser = SJX.fromValue(raw.enabledforuser, Boolean);
        },
        Boolean, 'leLinkStatus',
        Boolean, 'leEnabled',
        Boolean, 'leSyncComplete',
        String, 'leBaseUrl',
        Boolean, 'enabledForUser',

        Boolean, function isLEIntegrated(){
            return this.isLeEnabled() && this.isLeSyncComplete() && this.isEnabledForUser() && this.isLeLinkStatus();
            //return true;
        }

    ]);
});
