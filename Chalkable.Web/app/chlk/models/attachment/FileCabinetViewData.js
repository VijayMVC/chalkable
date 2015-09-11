REQUIRE('chlk.models.attachment.Attachment');
REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.id.AnnouncementAssignedAttributeId');
REQUIRE('chlk.models.announcement.BaseAnnouncementViewData');

NAMESPACE('chlk.models.attachment', function () {
    "use strict";

    /** @class chlk.models.attachment.FileCabinetViewData*/
    CLASS('FileCabinetViewData', [

        chlk.models.id.AnnouncementId, 'announcementId',
        chlk.models.announcement.AnnouncementTypeEnum, 'announcementType',

        chlk.models.common.PaginatedList, 'attachments',
        chlk.models.attachment.SortAttachmentType, 'sortType',
        String, 'filter',

        chlk.models.id.AnnouncementAssignedAttributeId, 'assignedAttributeId',

        [[chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum, chlk.models.common.PaginatedList, chlk.models.attachment.SortAttachmentType, String, chlk.models.id.AnnouncementAssignedAttributeId]],
        function $(announcementId_, announcementType_, attachments_, sortType_, filter_, assignedAttributeId_){
            BASE();
            if(announcementId_)
                this.setAnnouncementId(announcementId_);
            if(announcementType_)
                this.setAnnouncementType(announcementType_);
            if(attachments_)
                this.setAttachments(attachments_);
            if(sortType_)
                this.setSortType(sortType_);
            if(filter_)
                this.setFilter(filter_);
            if(assignedAttributeId_)
                this.setAssignedAttributeId(assignedAttributeId_);
        }
    ]);
});
