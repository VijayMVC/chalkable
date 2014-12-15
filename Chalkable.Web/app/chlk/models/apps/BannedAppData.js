REQUIRE('ria.serialize.SJX');
REQUIRE('ria.serialize.IDeserializable');
REQUIRE('chlk.models.common.Role');

NAMESPACE('chlk.models.apps', function () {
    "use strict";

    var SJX = ria.serialize.SJX;
    /** @class chlk.models.apps.BannedAppData*/
    CLASS(
        FINAL, UNSAFE, 'BannedAppData',IMPLEMENTS(ria.serialize.IDeserializable),  [
            VOID, function deserialize(raw){
                this.bannedBy = SJX.fromDeserializable(raw.bannedby, chlk.models.common.Role);
                this.banned = SJX.fromValue(data.banned, Boolean);
                this.unbannable = SJX.fromValue(data.unbannable, Boolean);
            },

            chlk.models.common.Role, 'bannedBy',
            Boolean, 'banned',
            Boolean, 'unbannable',

            function $(){
                BASE();
                this.setBanned(false);
                this.setUnbannable(true);
                this.setBannedBy(new chlk.models.common.Role());
            }
        ]);
});
