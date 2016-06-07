REQUIRE('chlk.models.student.StudentProfileSummaryViewData');
NAMESPACE('chlk.models.student', function(){
    "use strict";
    /**@class chlk.models.student.StudentProfileAssessmentViewData*/
    CLASS('StudentProfileAssessmentViewData', EXTENDS(chlk.models.student.StudentProfileSummaryViewData), [

    	chlk.models.apps.ExternalAttachAppViewData, 'attachAppViewData',

        [[
        	chlk.models.common.Role, 
        	chlk.models.student.StudentSummary, 
        	ArrayOf(chlk.models.people.Claim)
        ]],
            function $(role_, studentSummary_, claims_){
                BASE(role_, studentSummary_, claims_);
            }
        ]);
});