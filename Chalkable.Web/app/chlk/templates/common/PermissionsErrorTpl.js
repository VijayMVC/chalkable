REQUIRE('chlk.models.common.PermissionsError');
REQUIRE('chlk.templates.common.PageWithClasses');

NAMESPACE('chlk.templates.common',function(){
   "use strict";
    /**@class chlk.templates.common.PermissionsErrorTpl*/

    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/common/PermissionsError.jade')],
        [ria.templates.ModelBind(chlk.models.common.PermissionsError)],
        'PermissionsErrorTpl', EXTENDS(chlk.templates.common.PageWithClasses),[
            [ria.templates.ModelPropertyBind],
            Array, 'permissions',

            function getText(){
                var permissions = this.getPermissions().map(function(item){
                    if(Array.isArray(item)){
                        var arr = item.map(function(perm){
                            return perm.valueOf();
                        });
                        return arr.join(' or ');
                    }
                    return item.valueOf();
                });
                var res = 'You do not have permission to ' + permissions.join(', ');
                return res;
            }
    ]);
});