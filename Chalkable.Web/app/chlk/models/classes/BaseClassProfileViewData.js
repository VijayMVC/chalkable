REQUIRE('chlk.models.profile.BaseProfileViewData');

NAMESPACE('chlk.models.classes', function(){
   "use strict";
    /**@class chlk.models.classes.BaseClassProfileViewData*/
    CLASS('BaseClassProfileViewData', EXTENDS(chlk.models.profile.BaseProfileViewData),[

        [[chlk.models.common.Role, chlk.models.classes.Class, ArrayOf(chlk.models.people.Claim), Boolean]],
        function $(role_, clazz_, claims_, isAssignedToClass_){
            BASE(role_);
            if(clazz_)
                this._clazz = clazz_;
            if(claims_)
                this.setClaims(claims_);
            if(isAssignedToClass_)
                this.setAssignedToClass(isAssignedToClass_);
        },

        Boolean, 'assignedToClass',

        Object, function getClazz(){
            return this._clazz;
        }
    ])
});