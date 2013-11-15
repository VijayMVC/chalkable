REQUIRE('chlk.models.common.Role');

NAMESPACE('chlk.models.profile', function(){
   "use strict";

    /**@class chlk.models.profile.BaseProfileViewData*/
    CLASS('BaseProfileViewData',[

        chlk.models.common.RoleEnum, 'currentRoleId',

        Boolean, function isAdmin(){
            var role = this.getCurrentRoleId();
            return role == this._roleEnums.ADMINGRADE
                || role == this._roleEnums.ADMINEDIT
                || role == this._roleEnums.ADMINVIEW;
        },

        [[chlk.models.common.Role]],
        function $(role_){
            BASE();
            this._roleEnums = chlk.models.common.RoleEnum;
            if(role_)
                this.setCurrentRoleId(role_.getRoleId());
        }
    ]);
});