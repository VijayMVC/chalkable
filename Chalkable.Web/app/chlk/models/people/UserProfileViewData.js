REQUIRE('chlk.models.profile.BaseProfileViewData');
REQUIRE('chlk.models.people.ShortUserInfo');

NAMESPACE('chlk.models.people', function(){
    "use strict";
    /**@class chlk.models.people.UserProfileViewData*/
    CLASS('UserProfileViewData', EXTENDS(chlk.models.profile.BaseProfileViewData), [

        Object, function getUser(){return this._user;},

        VOID, function setUser_(user){
            if(user instanceof chlk.models.people.ShortUserInfo)
                return this._user = user;
            throw Exception('parameter should be instance of chlk.models.people.ShortUserInfo');
        },
        [[chlk.models.common.Role, chlk.models.people.ShortUserInfo]],
        function $(role_, user_){
            BASE(role_);
            if(user_)
                this._user = user_;
        }
    ]);
});