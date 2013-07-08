REQUIRE('chlk.activities.lib.TemplateActivity');
REQUIRE('chlk.templates.apps.AppCategories');

NAMESPACE('chlk.activities.apps', function () {

    /** @class chlk.activities.apps.AppCategoryListPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.BindTemplate(chlk.templates.apps.AppCategories)],
        'AppCategoryListPage', EXTENDS(chlk.activities.lib.TemplateActivity), [ ]);
});