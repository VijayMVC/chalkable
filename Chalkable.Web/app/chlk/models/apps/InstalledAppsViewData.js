REQUIRE('chlk.models.common.PaginatedList');
REQUIRE('chlk.models.common.BaseAttachViewData');

NAMESPACE('chlk.models.apps', function () {

    "use strict";
    /** @class chlk.models.apps.InstalledAppsViewData*/
    CLASS(
        'InstalledAppsViewData', EXTENDS(chlk.models.common.BaseAttachViewData), [

            chlk.models.common.PaginatedList, 'apps',

            [[chlk.models.common.AttachOptionsViewData, chlk.models.common.PaginatedList]],
            function $(options, apps){
                BASE(options);
                this.setApps(apps);
            }
        ]);
});
