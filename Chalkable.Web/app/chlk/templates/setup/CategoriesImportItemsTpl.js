REQUIRE('chlk.models.setup.CategoriesImportViewData');
REQUIRE('chlk.templates.ChlkTemplate');

NAMESPACE('chlk.templates.setup', function () {

    /** @class chlk.templates.setup.CategoriesImportItemsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/setup/CategoriesImportItems.jade')],
        [ria.templates.ModelBind(chlk.models.setup.CategoriesImportViewData)],
        'CategoriesImportItemsTpl', EXTENDS(chlk.templates.setup.CategoriesImportTpl), [

        ])
});
