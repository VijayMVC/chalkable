REQUIRE('chlk.activities.setup.CategoriesSetupPage');
REQUIRE('chlk.templates.setup.CommentsSetupTpl');

NAMESPACE('chlk.activities.setup', function () {

    /** @class chlk.activities.setup.CommentsSetupPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.setup.CommentsSetupTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.setup.CommentsSetupTpl, '', null, ria.mvc.PartialUpdateRuleActions.Replace)],
        'CommentsSetupPage', EXTENDS(chlk.activities.setup.CategoriesSetupPage), [

        ]);
});