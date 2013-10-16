REQUIRE('chlk.models.people.UserProfileViewData');
REQUIRE('chlk.models.people.PersonSummary');

NAMESPACE('chlk.models.people', function(){
    "use strict";
    /**@class chlk.models.people.UserProfileSummaryViewData*/

    CLASS('UserProfileSummaryViewData', EXTENDS(chlk.models.people.UserProfileViewData),[

        [[chlk.models.common.Role, chlk.models.people.PersonSummary]],
        function $(role_, userSummary_){
            BASE(role_, userSummary_);
        }
    ]);
});