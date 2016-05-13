REQUIRE('chlk.models.apps.Application');
REQUIRE('chlk.models.apps.MyAppsViewData');
REQUIRE('chlk.models.common.PaginatedList');

NAMESPACE('chlk.templates.apps', function () {

    /** @class chlk.templates.apps.MyApps*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/my-apps.jade')],
        [ria.templates.ModelBind(chlk.models.apps.MyAppsViewData)],
        'MyApps', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.common.PaginatedList, 'apps',


            Boolean, function canDisableApp(){
                var role = this.getUserRole();
                return role.isSysAdmin() || role.isAdmin();
            },

            [[chlk.models.apps.Application]],
            String, function bannedCssClass(app){
                if(this.canDisableApp()){
                    if(app.isBannedForDistrict()) return 'banned';
                    if(app.isPartiallyBanned()) return 'partial-banned';
                }
                return '';
            },
        ])
});