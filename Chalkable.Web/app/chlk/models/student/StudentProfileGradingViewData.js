REQUIRE('chlk.models.people.UserProfileViewData');
REQUIRE('chlk.models.student.StudentGradingInfo');
REQUIRE('chlk.models.schoolYear.GradingPeriod');

NAMESPACE('chlk.models.student', function(){
    "use strict";
    /**@class chlk.models.student.StudentProfileGradingViewData*/
    CLASS('StudentProfileGradingViewData', EXTENDS(chlk.models.people.UserProfileViewData.OF(chlk.models.student.StudentGradingInfo)), [

        chlk.models.schoolYear.GradingPeriod, 'gradingPeriod',

        [[chlk.models.common.Role
            , chlk.models.student.StudentGradingInfo
            , chlk.models.schoolYear.GradingPeriod
            , ArrayOf(chlk.models.people.Claim)
        ]],
        function $(role_, studentGradingInfo_, gradingPeriod_, claims_){
            BASE(role_, studentGradingInfo_, claims_);
            if(gradingPeriod_)
                this.setGradingPeriod(gradingPeriod_);
        }
    ]);
});