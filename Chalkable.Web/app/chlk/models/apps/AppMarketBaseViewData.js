REQUIRE('chlk.models.apps.AppMarketApplication');

NAMESPACE('chlk.models.apps', function () {
    "use strict";


    /** @class chlk.models.apps.AppMarketBaseViewData*/
    CLASS(
        'AppMarketBaseViewData', [
            Number, 'currentBalance',
            ArrayOf(chlk.models.apps.AppCategory), 'categories',
            ArrayOf(chlk.models.apps.AppGradeLevel), 'gradeLevels',

            [[
                ArrayOf(chlk.models.apps.AppCategory),
                ArrayOf(chlk.models.apps.AppGradeLevel),
                Number
            ]],
            function $(categories, gradelelevels, balance){
                BASE();
                this.setCurrentBalance(balance);
                this.setCategories(categories);
                this.setGradeLevels(gradelelevels);
            }

        ]);


});
