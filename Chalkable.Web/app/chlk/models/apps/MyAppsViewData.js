REQUIRE('chlk.models.common.PaginatedList');

NAMESPACE('chlk.models.apps', function () {
    "use strict";
    /** @class chlk.models.apps.MyAppsViewData*/
    CLASS(
        'MyAppsViewData', [
            chlk.models.common.PaginatedList, 'apps',
            Boolean, 'editable',

            [[chlk.models.common.PaginatedList, Boolean]],
            function $(apps, editable){
                BASE();
                this.setApps(apps);
                this.setEditable(editable);
            }
        ]);


});
