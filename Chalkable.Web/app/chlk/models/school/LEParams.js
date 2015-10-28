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
            this.issueLECreditsEnabled = SJX.fromValue(raw.issuelecreditsenabled, Boolean);
            this.leAccessEnabled = SJX.fromValue(raw.leaccessenabled, Boolean);
        },
        Boolean, 'leLinkStatus',
        Boolean, 'leEnabled',
        Boolean, 'leSyncComplete',
        String, 'leBaseUrl',
        Boolean, 'enabledForUser',
        Boolean, 'issueLECreditsEnabled',
        Boolean, 'leAccessEnabled',

        Boolean, function isLEIntegrated(){
            return this.isLeEnabled() && this.isLeSyncComplete() && this.isIssueLECreditsEnabled() && this.isLeLinkStatus();
            //return true;
        },

        Boolean, function isIntegratedSignOn(){
            return this.isLeEnabled() && this.isLeSyncComplete() && this.isLeAccessEnabled() && this.isLeLinkStatus();
            //return true;
        },





    ]);
});
