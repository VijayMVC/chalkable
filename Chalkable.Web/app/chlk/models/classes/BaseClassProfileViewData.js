REQUIRE('chlk.models.profile.BaseProfileViewData');

NAMESPACE('chlk.models.classes', function(){
   "use strict";
    /**@class chlk.models.classes.BaseClassProfileViewData*/
    CLASS('BaseClassProfileViewData', EXTENDS(chlk.models.profile.BaseProfileViewData),[

        [[chlk.models.common.Role, chlk.models.classes.Class]],
        function $(role_, clazz_){
            BASE(role_);
            if(clazz_ && !(clazz_ instanceof chlk.models.classes.Class))
                throw new Exception('clazz should be instance of chlk.models.classes.Class');
            this._clazz = clazz_;
        },
        Object, function getClazz(){return this._clazz;}
    ])
});