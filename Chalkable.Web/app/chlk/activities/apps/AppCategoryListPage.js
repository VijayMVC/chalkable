REQUIRE('ria.mvc.TemplateActivity');
REQUIRE('chlk.templates.apps.AppCategories');

NAMESPACE('chlk.activities.apps', function () {

    /** @class chlk.activities.apps.AppCategoryListPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.apps.AppCategories)],
        'AppCategoryListPage', EXTENDS(ria.mvc.TemplateActivity), [ ]);
});