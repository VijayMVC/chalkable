REQUIRE('chlk.models.people.UserProfileViewData');
REQUIRE('chlk.models.student.StudentGradingInfo');

NAMESPACE('chlk.models.student', function(){
    "use strict";
    /**@class chlk.models.student.StudentProfileGradingViewData*/
    CLASS('StudentProfileGradingViewData', EXTENDS(chlk.models.people.UserProfileViewData), [

        [[chlk.models.common.Role, chlk.models.student.StudentGradingInfo]],
        function $(role_, studentGradingInfo_){
            BASE(role_, studentGradingInfo_);
        }
    ]);
});