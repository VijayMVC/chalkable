REQUIRE('chlk.models.common.PaginatedList');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.models.apps', function () {
    "use strict";
    /** @class chlk.models.apps.InstalledAppsViewData*/
    CLASS(
        'InstalledAppsViewData', [

            chlk.models.id.SchoolPersonId, 'teacherId',
            chlk.models.id.AnnouncementId, 'announcementId',
            chlk.models.id.ClassId, 'classId',
            chlk.models.common.PaginatedList, 'apps',
            String, 'appUrlAppend',

            [[chlk.models.id.SchoolPersonId, chlk.models.id.AnnouncementId, chlk.models.id.ClassId
                , chlk.models.common.PaginatedList, String]],
            function $(teacherId, announcementId, classId, apps, appUrlAppend){
                BASE();
                this.setTeacherId(teacherId);
                this.setAnnouncementId(announcementId);
                this.setApps(apps);
                this.setClassId(classId);
                this.setAppUrlAppend(appUrlAppend);
            }
        ]);
});
