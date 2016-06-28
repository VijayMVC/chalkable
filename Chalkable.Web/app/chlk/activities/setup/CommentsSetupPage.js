REQUIRE('chlk.activities.setup.CategoriesSetupPage');
REQUIRE('chlk.templates.setup.CommentsSetupTpl');

NAMESPACE('chlk.activities.setup', function () {

    /** @class chlk.activities.setup.CommentsSetupPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.setup.CommentsSetupTpl)],
        'CommentsSetupPage', EXTENDS(chlk.activities.setup.CategoriesSetupPage), [

        ]);
});