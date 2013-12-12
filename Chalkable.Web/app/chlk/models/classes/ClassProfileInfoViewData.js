REQUIRE('chlk.models.profile.BaseProfileViewData');
REQUIRE('chlk.models.classes.ClassInfo');

NAMESPACE('chlk.models.classes', function(){
    "use strict";

    /**@class chlk.models.classes.ClassProfileInfoViewData*/
    CLASS('ClassProfileInfoViewData', EXTENDS(chlk.models.classes.BaseClassProfileViewData),[

        [[chlk.models.common.Role, chlk.models.classes.ClassInfo, ArrayOf(chlk.models.people.Claim)]],
        function $(role_, classInfo_, claims_){
            BASE(role_, classInfo_, claims_);
        }
    ]);
});