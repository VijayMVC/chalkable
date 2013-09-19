REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.developer.ApiExplorer');

NAMESPACE('chlk.activities.developer', function () {

    /** @class chlk.activities.developer.ApiExplorerPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.developer.ApiExplorer)],
        'ApiExplorerPage', EXTENDS(chlk.activities.lib.TemplatePage), [

        ]);
});