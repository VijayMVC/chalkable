REQUIRE('chlk.models.people.UserProfileViewData');
REQUIRE('chlk.models.student.StudentGradingInfo');
REQUIRE('chlk.models.schoolYear.MarkingPeriod');

NAMESPACE('chlk.models.student', function(){
    "use strict";
    /**@class chlk.models.student.StudentProfileGradingViewData*/
    CLASS('StudentProfileGradingViewData', EXTENDS(chlk.models.people.UserProfileViewData.OF(chlk.models.student.StudentGradingInfo)), [

        chlk.models.schoolYear.MarkingPeriod, 'markingPeriod',

        [[chlk.models.common.Role
            , chlk.models.student.StudentGradingInfo
            , chlk.models.schoolYear.MarkingPeriod
        ]],
        function $(role_, studentGradingInfo_, markingPeriod_){
            BASE(role_, studentGradingInfo_);
            if(markingPeriod_)
                this.setMarkingPeriod(markingPeriod_);
        }
    ]);
});