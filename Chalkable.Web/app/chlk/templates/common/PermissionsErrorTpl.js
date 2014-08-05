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
            ArrayOf(chlk.models.people.UserPermissionEnum), 'permissions',

            function getText(){
                var permissions = this.getPermissions().map(function(item){
                    return item.valueOf();
                });
                var res = 'You do not have permission to ' + permissions.join(', ');
                return res;
            }
    ]);
});