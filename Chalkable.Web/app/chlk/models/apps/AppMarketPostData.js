REQUIRE('chlk.models.apps.AppPriceType');
REQUIRE('chlk.models.apps.AppSortingMode');

NAMESPACE('chlk.models.apps', function () {
    "use strict";

    /** @class chlk.models.apps.AppMarketPostData*/
    CLASS(
        'AppMarketPostData', [
            String, 'selectedCategories',

            [ria.serialize.SerializeProperty('gradeLevelsSelectedValues')],
            String, 'gradeLevels',
            Number, 'start',

            String, 'filter',
            Number, 'priceType',
            Number, 'sortingMode',
            Boolean, 'scroll'
        ]);
});
