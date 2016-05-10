REQUIRE('chlk.models.people.UserProfileViewData');
REQUIRE('chlk.models.people.ShortUserInfo');

NAMESPACE('chlk.models.people', function(){
   "use strict";

    /**@class chlk.models.people.UserProfileAppsViewData*/
    CLASS('UserProfileAppsViewData', EXTENDS(chlk.models.people.UserProfileViewData.OF(chlk.models.people.ShortUserInfo)),[

        [[chlk.models.common.Role, chlk.models.people.ShortUserInfo, ArrayOf(chlk.models.people.Claim)]],
        function $(role_, user_, claims_){
            BASE(role_, user_, claims_);
        }
    ])
});