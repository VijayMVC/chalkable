REQUIRE('chlk.models.profile.BaseProfileViewData');
REQUIRE('chlk.models.people.ShortUserInfo');

NAMESPACE('chlk.models.people', function(){
    "use strict";
    /**@class chlk.models.people.UserProfileViewData*/
    CLASS(
        GENERIC('TUser', ClassOf(chlk.models.people.ShortUserInfo)),
        'UserProfileViewData', EXTENDS(chlk.models.profile.BaseProfileViewData), [

        TUser, function getUser(){return this._user;},
        [[TUser]],
        VOID, function setUser_(user){
                return this._user = user;
        },
        [[chlk.models.common.Role, TUser]],
        function $(role_, user_){
            BASE(role_);
            if(user_)
                this._user = user_;
        }
    ]);
});