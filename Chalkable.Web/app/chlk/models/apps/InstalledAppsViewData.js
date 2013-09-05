REQUIRE('chlk.models.common.PaginatedList');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.id.AnnouncementId');

NAMESPACE('chlk.models.apps', function () {
    "use strict";
    /** @class chlk.models.apps.InstalledAppsViewData*/
    CLASS(
        'InstalledAppsViewData', [

            chlk.models.id.SchoolPersonId, 'teacherId',
            chlk.models.id.AnnouncementId, 'announcementId',
            chlk.models.common.PaginatedList, 'apps',

            [[chlk.models.id.SchoolPersonId, chlk.models.id.AnnouncementId, chlk.models.common.PaginatedList]],
            function $(teacherId, announcementId, apps){
                this.setTeacherId(teacherId);
                this.setAnnouncementId(announcementId);
                this.setApps(apps);
            }
        ]);
});
