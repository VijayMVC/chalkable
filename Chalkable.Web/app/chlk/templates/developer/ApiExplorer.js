REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.developer.ApiExplorerViewData');

NAMESPACE('chlk.templates.developer', function () {
    /** @class chlk.templates.developer.ApiExplorer*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/developer/api-explorer.jade')],
        [ria.templates.ModelBind(chlk.models.developer.ApiExplorerViewData)],
        'ApiExplorer', EXTENDS(chlk.templates.JadeTemplate), [])
});