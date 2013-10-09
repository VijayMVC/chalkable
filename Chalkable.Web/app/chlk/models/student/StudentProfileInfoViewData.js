REQUIRE('chlk.models.people.UserProfileInfoViewData');
REQUIRE('chlk.models.student.StudentInfo');

NAMESPACE('chlk.models.student', function(){
    "use strict";
    /**@class chlk.models.student.StudentProfileInfoViewData*/
    CLASS('StudentProfileInfoViewData', EXTENDS(chlk.models.people.UserProfileInfoViewData), [

        [[chlk.models.common.Role, chlk.models.student.StudentInfo]],
        function $(role_, studentInfo){
            BASE(role_, studentInfo);
        }
    ]);
});