REQUIRE('chlk.models.classes.BaseClassProfileViewData');
REQUIRE('chlk.models.classes.ClassSummary');

NAMESPACE('chlk.models.classes', function(){
    "use strict";

    /**@class chlk.models.classes.ClassProfileSummaryViewData*/
    CLASS('ClassProfileSummaryViewData', EXTENDS(chlk.models.classes.BaseClassProfileViewData),[

        [[chlk.models.common.Role, chlk.models.classes.ClassSummary]],
        function $(role_, classInfo_){
            BASE(role_, classInfo_);
        }
    ]);
});