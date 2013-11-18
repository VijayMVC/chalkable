REQUIRE('chlk.models.classes.BaseClassProfileViewData');
REQUIRE('chlk.models.classes.ClassApps');

NAMESPACE('chlk.models.classes', function(){
    "use strict";
    /**@class chlk.models.classes.ClassProfileAppsViewData*/
    CLASS('ClassProfileAppsViewData', EXTENDS(chlk.models.classes.BaseClassProfileViewData), [

        [[chlk.models.common.Role, chlk.models.classes.ClassApps]],
        function $(role_, classApps_){
            BASE(role_, classApps_);
        }
    ]);
});