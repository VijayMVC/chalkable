REQUIRE('chlk.models.classes.BaseClassProfileViewData');
REQUIRE('chlk.models.classes.Class');

NAMESPACE('chlk.models.classes', function(){
    "use strict";
    /**@class chlk.models.classes.ClassProfileAppsViewData*/
    CLASS('ClassProfileAppsViewData', EXTENDS(chlk.models.classes.BaseClassProfileViewData), [

        [[chlk.models.common.Role, chlk.models.classes.Class, ArrayOf(chlk.models.people.Claim), Boolean]],
        function $(role_, class_, claims_, isAssignedToClass_){
            BASE(role_, class_, claims_, isAssignedToClass_);
        }
    ]);
});