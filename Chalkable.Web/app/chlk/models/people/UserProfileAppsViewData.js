REQUIRE('chlk.models.people.UserProfileViewData');
REQUIRE('chlk.models.people.PersonApps');

NAMESPACE('chlk.models.people', function(){
   "use strict";

    /**@class chlk.models.people.UserProfileAppsViewData*/
    CLASS('UserProfileAppsViewData', EXTENDS(chlk.models.people.UserProfileViewData.OF(chlk.models.people.PersonApps)),[

        [[chlk.models.common.Role, chlk.models.people.PersonApps]],
        function $(role_, user_){
            BASE(role_, user_);
        }
    ])
});