REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.developer.ApiExplorerTpl');

NAMESPACE('chlk.activities.developer', function () {

    /** @class chlk.activities.developer.ApiExplorerPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.developer.ApiExplorerTpl)],
        'ApiExplorerPage', EXTENDS(chlk.activities.lib.TemplatePage), [

        ]);
});