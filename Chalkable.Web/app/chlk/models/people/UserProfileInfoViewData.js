REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.people.UserProfileViewData');

NAMESPACE('chlk.models.people', function(){
    "use strict";
    /**@class chlk.models.people.UserProfileInfoViewData*/
    CLASS('UserProfileInfoViewData', EXTENDS(chlk.models.people.UserProfileViewData.OF(chlk.models.people.User)), [

        [[chlk.models.common.Role, chlk.models.people.User, ArrayOf(chlk.models.people.Claim)]],
        function $(role_, user_, claims_){
            BASE(role_, user_, claims_);
        }
    ]);
});