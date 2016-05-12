REQUIRE('chlk.models.people.UserProfileViewData');
REQUIRE('chlk.models.people.ShortUserInfo');
REQUIRE('chlk.models.apps.Application');

NAMESPACE('chlk.models.people', function(){
   "use strict";

    /**@class chlk.models.people.UserProfileAppsViewData*/
    CLASS('UserProfileAppsViewData', EXTENDS(chlk.models.people.UserProfileViewData.OF(chlk.models.people.ShortUserInfo)),[

        ArrayOf(chlk.models.apps.Application), 'applications',

        [[chlk.models.common.Role, chlk.models.people.PersonApps, ArrayOf(chlk.models.people.Claim)]],
        function $(role_, user_, claims_){
            BASE(role_, user_, claims_);
            this.applications = user_.getApplications();
        }
    ])
});