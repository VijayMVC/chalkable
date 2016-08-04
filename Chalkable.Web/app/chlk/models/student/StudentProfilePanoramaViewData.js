REQUIRE('chlk.models.people.UserProfileInfoViewData');
REQUIRE('chlk.models.student.StudentPanoramaViewData');

NAMESPACE('chlk.models.student', function(){
    "use strict";
    /**@class chlk.models.student.StudentProfilePanoramaViewData*/
    CLASS('StudentProfilePanoramaViewData', EXTENDS(chlk.models.people.UserProfileViewData.OF(chlk.models.student.StudentPanoramaViewData)), [

        [[chlk.models.common.Role, chlk.models.student.StudentPanoramaViewData, ArrayOf(chlk.models.people.Claim)]],
        function $(role_, studentInfo_, claims_){
            BASE(role_, studentInfo_, claims_);
        }
    ]);
});