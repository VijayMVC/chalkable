REQUIRE('chlk.models.people.UserProfileViewData');
REQUIRE('chlk.models.student.StudentSummary');
NAMESPACE('chlk.models.student', function(){
    "use strict";
    /**@class chlk.models.student.StudentProfileSummaryViewData*/
    CLASS('StudentProfileSummaryViewData', EXTENDS(chlk.models.people.UserProfileViewData.OF(chlk.models.student.StudentSummary)), [

        [[chlk.models.common.Role, chlk.models.student.StudentSummary, ArrayOf(chlk.models.people.Claim)]],
            function $(role_, studentSummary_, claims_){
                BASE(role_, studentSummary_, claims_);
            }
        ]);
});