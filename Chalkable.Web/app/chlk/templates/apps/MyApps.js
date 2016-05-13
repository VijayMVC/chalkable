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
            Boolean, function isPartialBanned(app){
               return (app.getBannedSchoolIds() || []).length > 0;
            },

            [[chlk.models.apps.Application]],
            String, function bannedCssClass(app){
                if(app.isBanned()) return 'banned';
                if(this.isPartialBanned(app)) return 'partial-banned';
                return '';
            },

            [[chlk.models.apps.Application]],
            Boolean, function isBannedFromCurrentSchool(app){
                //TODO impl
                return app.isBanned();
            }
        ])
});