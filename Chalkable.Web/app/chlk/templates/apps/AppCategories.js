REQUIRE('chlk.templates.PaginatedList');
REQUIRE('chlk.models.apps.AppCategory');

NAMESPACE('chlk.templates.apps', function () {

    /** @class chlk.templates.apps.AppCategories*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/app-categories.jade')],
        [ria.templates.ModelBind(chlk.models.common.PaginatedList)],
        'AppCategories', EXTENDS(chlk.templates.PaginatedList), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.apps.AppCategory), 'items'
        ])
});