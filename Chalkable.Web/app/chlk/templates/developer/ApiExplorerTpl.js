REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.api.ApiExplorerViewData');

NAMESPACE('chlk.templates.developer', function () {
    /** @class chlk.templates.developer.ApiExplorerTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/developer/api-explorer.jade')],
        [ria.templates.ModelBind(chlk.models.api.ApiExplorerViewData)],
        'ApiExplorerTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.api.ApiRoleInfo, 'apiInfo',

            [ria.templates.ModelPropertyBind],
            String, 'appSecretKey',

            [ria.templates.ModelPropertyBind],
            ArrayOf(String), 'apiRoles',


            String, function getRubyExample(){

            },

            String, function getPythonExample(){

            },

            String, function getCurlExample(){

            }
        ])
});