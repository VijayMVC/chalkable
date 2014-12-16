REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.api.ApiResponse');

NAMESPACE('chlk.templates.developer', function () {
    /** @class chlk.templates.developer.ApiExplorerResponseTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/developer/api-explorer-response.jade')],
        [ria.templates.ModelBind(chlk.models.api.ApiResponse)],
        'ApiExplorerResponseTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            String, 'apiFormId',

            [ria.templates.ModelPropertyBind],
            String, 'responseData'
        ])
});