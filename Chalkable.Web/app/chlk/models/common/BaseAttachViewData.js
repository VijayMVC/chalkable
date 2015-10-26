REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.models.common', function () {

    "use strict";
    /** @class chlk.models.common.BaseAttachViewData*/
    CLASS(
        'BaseAttachViewData', [

            chlk.models.id.AnnouncementId, 'announcementId',
            chlk.models.id.ClassId, 'classId',
            chlk.models.id.AppId, 'assessmentAppId',
            String, 'announcementTypeName',

            chlk.models.announcement.AnnouncementTypeEnum, 'announcementType',

            Boolean, 'fileCabinetEnabled',
            Boolean, 'standardAttachEnabled',
            Boolean, 'showApps',

            [[
                chlk.models.id.AnnouncementId,
                chlk.models.announcement.AnnouncementTypeEnum
            ]],
            function $createForStudent(announcementId, announcementType){
                BASE();
                this.setAnnouncementId(announcementId);
                this.setFileCabinetEnabled(false);
                this.setStandardAttachEnabled(false);
                this.setShowApps(false);
                this.setAnnouncementType(announcementType);
            }
        ]);
});
