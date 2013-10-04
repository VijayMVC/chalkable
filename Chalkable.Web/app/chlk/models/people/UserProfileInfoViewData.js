REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.people.UserProfileViewData');

NAMESPACE('chlk.models.people', function(){
    "use strict";
    /**@class chlk.models.people.UserProfileInfoViewData*/
    CLASS('UserProfileInfoViewData', EXTENDS(chlk.models.people.UserProfileViewData), [

        [[chlk.models.common.Role, chlk.models.people.User]],
        function $(role_, user_){
            BASE(role_, user_);
        }
    ]);
});