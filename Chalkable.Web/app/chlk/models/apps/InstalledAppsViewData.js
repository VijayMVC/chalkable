REQUIRE('chlk.models.common.PaginatedList');
REQUIRE('chlk.models.id.SchoolPersonId');

NAMESPACE('chlk.models.apps', function () {
    "use strict";
    /** @class chlk.models.apps.InstalledAppsViewData*/
    CLASS(
        'InstalledAppsViewData', [

            chlk.models.id.SchoolPersonId, 'teacherId',
            chlk.models.common.PaginatedList, 'apps',

            [[chlk.models.id.SchoolPersonId, chlk.models.common.PaginatedList]],
            function $(teacherId, apps){
                this.setTeacherId(teacherId);
                this.setApps(apps);
            }
        ]);
});
