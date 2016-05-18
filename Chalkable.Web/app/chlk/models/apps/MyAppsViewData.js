REQUIRE('chlk.models.common.PaginatedList');

NAMESPACE('chlk.models.apps', function () {
    "use strict";
    /** @class chlk.models.apps.MyAppsViewData*/
    CLASS(
        'MyAppsViewData', [
            chlk.models.common.PaginatedList, 'apps',

            [[chlk.models.common.PaginatedList]],
            function $(apps){
                BASE();
                this.setApps(apps);
            }
        ]);
});
