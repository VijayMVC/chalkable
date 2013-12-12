REQUIRE('chlk.models.classes.BaseClassProfileViewData');
REQUIRE('chlk.models.classes.ClassApps');

NAMESPACE('chlk.models.classes', function(){
    "use strict";
    /**@class chlk.models.classes.ClassProfileAppsViewData*/
    CLASS('ClassProfileAppsViewData', EXTENDS(chlk.models.classes.BaseClassProfileViewData), [

        [[chlk.models.common.Role, chlk.models.classes.ClassApps, ArrayOf(chlk.models.people.Claim)]],
        function $(role_, classApps_, claims_){
            BASE(role_, classApps_, claims_);
        }
    ]);
});