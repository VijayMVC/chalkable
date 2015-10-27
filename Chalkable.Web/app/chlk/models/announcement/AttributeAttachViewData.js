REQUIRE('chlk.models.common.BaseAttachViewData');
REQUIRE('chlk.models.id.AnnouncementAssignedAttributeId');

NAMESPACE('chlk.models.announcement', function () {

    "use strict";
    /** @class chlk.models.announcement.AttributeAttachViewData*/
    CLASS(
        'AttributeAttachViewData', EXTENDS(chlk.models.common.BaseAttachViewData), [

            chlk.models.id.AnnouncementAssignedAttributeId, 'assignedAttributeId',

            [[
                chlk.models.id.AnnouncementId,
                chlk.models.announcement.AnnouncementTypeEnum,
                chlk.models.id.AnnouncementAssignedAttributeId
            ]],
            function $(announcementId, announcementType, assignedAttributeId_){
                BASE();
                this.setAnnouncementId(announcementId);
                this.setAnnouncementType(announcementType);
                this.setFileCabinetEnabled(true);
                this.setStandardAttachEnabled(false);
                this.setShowApps(false);
                if(assignedAttributeId_)
                    this.setAssignedAttributeId(assignedAttributeId_);
            }
        ]);
});
