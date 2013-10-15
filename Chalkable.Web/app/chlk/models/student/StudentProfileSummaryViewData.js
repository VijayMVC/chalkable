REQUIRE('chlk.models.people.UserProfileViewData');
REQUIRE('chlk.models.student.StudentSummary');
NAMESPACE('chlk.models.student', function(){
    "use strict";
    /**@class chlk.models.student.StudentProfileSummaryViewData*/
    CLASS('StudentProfileSummaryViewData', EXTENDS(chlk.models.people.UserProfileViewData), [

        [[chlk.models.common.Role, chlk.models.student.StudentSummary]],
            function $(role_, studentSummary_){
                BASE(role_, studentSummary_);
            }
        ]);
});