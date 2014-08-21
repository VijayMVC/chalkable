REQUIRE('chlk.models.profile.BaseProfileViewData');

NAMESPACE('chlk.models.classes', function(){
   "use strict";
    /**@class chlk.models.classes.BaseClassProfileViewData*/
    CLASS('BaseClassProfileViewData', EXTENDS(chlk.models.profile.BaseProfileViewData),[

        [[chlk.models.common.Role, chlk.models.classes.Class, ArrayOf(chlk.models.people.Claim)]],
        function $(role_, clazz_, claims_){
            BASE(role_);
            if(clazz_)
                this._clazz = clazz_;
            if(claims_)
                this.setClaims(claims_);
        },
        Object, function getClazz(){
            return this._clazz;
        }
    ])
});