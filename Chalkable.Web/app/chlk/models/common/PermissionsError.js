REQUIRE('chlk.models.common.PageWithClasses');
REQUIRE('chlk.models.people.Claim');

NAMESPACE('chlk.models.common', function(){
   "use strict";
    /**@class chlk.models.common.PermissionsError*/

    CLASS('PermissionsError', EXTENDS(chlk.models.common.PageWithClasses), [

        Array, 'permissions',

        [[chlk.models.classes.ClassesForTopBar, chlk.models.id.ClassId, Array]],
        function $(classes_, classId_, permissions_){
            BASE(classes_, classId_);
            if(permissions_)
                this.setPermissions(permissions_);
        }
    ]);
});