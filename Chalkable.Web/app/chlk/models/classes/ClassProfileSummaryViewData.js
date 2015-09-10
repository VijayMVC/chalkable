REQUIRE('chlk.models.classes.BaseClassProfileViewData');
REQUIRE('chlk.models.classes.ClassSummary');

NAMESPACE('chlk.models.classes', function(){
    "use strict";

    /**@class chlk.models.classes.ClassProfileSummaryViewData*/
    CLASS('ClassProfileSummaryViewData', EXTENDS(chlk.models.classes.BaseClassProfileViewData),[

        [[chlk.models.common.Role, chlk.models.classes.ClassSummary, ArrayOf(chlk.models.people.Claim), Boolean]],
        function $(role_, classInfo_, claims_, isAssignedToClass_){
            BASE(role_, classInfo_, claims_, isAssignedToClass_);
        }
    ]);
});