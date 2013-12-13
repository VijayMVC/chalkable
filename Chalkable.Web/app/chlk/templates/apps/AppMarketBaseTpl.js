REQUIRE('chlk.models.common.PaginatedList');
REQUIRE('chlk.models.apps.AppMarketBaseViewData');
REQUIRE('chlk.models.apps.AppSortingMode');
REQUIRE('chlk.models.apps.AppMarketApplication');
REQUIRE('chlk.models.apps.AppCategory');
REQUIRE('chlk.models.apps.AppGradeLevel');
REQUIRE('chlk.templates.apps.AppMarketSearchBoxTpl');

NAMESPACE('chlk.templates.apps', function () {

    ASSET('~/assets/jade/activities/apps/AppMarketBase.jade')();
    /** @class chlk.templates.apps.AppMarketBaseTpl*/
    CLASS(

        [ria.templates.ModelBind(chlk.models.apps.AppMarketBaseViewData)],
        'AppMarketBaseTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            Number, 'currentBalance',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.apps.AppCategory), 'categories',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.apps.AppGradeLevel), 'gradeLevels'

        ])
});