REQUIRE('chlk.models.people.UserProfileInfoViewData');
REQUIRE('chlk.models.student.StudentInfo');

NAMESPACE('chlk.models.student', function(){
    "use strict";
    /**@class chlk.models.student.StudentProfileInfoViewData*/
    CLASS('StudentProfileInfoViewData', EXTENDS(chlk.models.people.UserProfileViewData.OF(chlk.models.student.StudentInfo)), [

        [[chlk.models.common.Role, chlk.models.student.StudentInfo]],
        function $(role_, studentInfo_){
            BASE(role_, studentInfo_);
        }
    ]);
});