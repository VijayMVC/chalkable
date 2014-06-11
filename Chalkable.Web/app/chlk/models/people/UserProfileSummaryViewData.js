REQUIRE('chlk.models.people.UserProfileViewData');
REQUIRE('chlk.models.people.PersonSummary');

NAMESPACE('chlk.models.people', function(){
    "use strict";
    /**@class chlk.models.people.UserProfileSummaryViewData*/

    CLASS('UserProfileSummaryViewData', EXTENDS(chlk.models.people.UserProfileViewData.OF(chlk.models.people.PersonSummary)),[

        [[chlk.models.common.Role, chlk.models.people.PersonSummary, ArrayOf(chlk.models.people.Claim)]],
        function $(role_, userSummary_, claims_){
            BASE(role_, userSummary_, claims_);
        }
    ]);
});