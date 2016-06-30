REQUIRE('chlk.models.apps.AppPriceType');
REQUIRE('chlk.models.apps.AppSortingMode');
REQUIRE('chlk.models.id.SchoolPersonId');

NAMESPACE('chlk.models.apps', function () {
    "use strict";

    /** @class chlk.models.apps.GetAppsPostData*/
    CLASS(
        'GetAppsPostData', [

            Number, 'start',
            String, 'filter',

            [ria.serialize.SerializeProperty('developerid')],
            chlk.models.id.SchoolPersonId, 'developerId',
            Number, 'state'
    ]);
});
