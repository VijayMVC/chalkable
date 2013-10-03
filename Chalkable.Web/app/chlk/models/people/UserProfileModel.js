REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.common.ActionLinkModel');

NAMESPACE('chlk.models.people', function(){
    "use strict";
    /**@class chlk.models.people.UserProfileModel*/
    CLASS('UserProfileModel', EXTENDS(chlk.models.profile.BaseProfileViewData), [
        chlk.models.people.User, 'user',
        ArrayOf(chlk.models.common.ActionLinkModel), 'actionLinksData',

        [[chlk.models.common.Role, chlk.models.people.User, ArrayOf(chlk.models.common.ActionLinkModel)]],
        function $(role_, user_, actionLinksData_){
            BASE(role_); //todo : remove actionLinksData
            if(user_)
                this.setUser(user_);
            if(actionLinksData_)
                this.setActionLinksData(actionLinksData_);
        }
    ]);
});