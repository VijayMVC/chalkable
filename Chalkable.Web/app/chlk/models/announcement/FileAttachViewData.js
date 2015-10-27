REQUIRE('chlk.models.common.BaseAttachViewData');
REQUIRE('chlk.models.id.AnnouncementAssignedAttributeId');

NAMESPACE('chlk.models.announcement', function () {

    "use strict";
    /** @class chlk.models.announcement.FileAttachViewData*/
    CLASS(
        'FileAttachViewData', EXTENDS(chlk.models.common.BaseAttachViewData), [



            [[
                chlk.models.id.AnnouncementId,
                chlk.models.announcement.AnnouncementTypeEnum,
                chlk.models.id.ClassId,
                chlk.models.id.AnnouncementAssignedAttributeId
            ]],
            function $(announcementId, announcementType, classId_, assignedAttributeId_){
                BASE();
                this.setAnnouncementId(announcementId);
                this.setAnnouncementType(announcementType);
                classId_ && this.setClassId(classId_);
                this.setFileCabinetEnabled(true);
                this.setStandardAttachEnabled(false);
                this.setShowApps(false);
                if(assignedAttributeId_)
                    this.setAssignedAttributeId(assignedAttributeId_);
            }
        ]);
});
