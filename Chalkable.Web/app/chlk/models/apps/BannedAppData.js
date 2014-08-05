REQUIRE('chlk.models.common.Role');

NAMESPACE('chlk.models.apps', function () {
    "use strict";

    /** @class chlk.models.apps.BannedAppData*/
    CLASS(
        'BannedAppData', [
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
