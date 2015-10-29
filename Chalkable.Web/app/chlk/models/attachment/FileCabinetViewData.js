REQUIRE('chlk.models.attachment.Attachment');
REQUIRE('chlk.models.common.BaseAttachViewData');
REQUIRE('chlk.models.announcement.BaseAnnouncementViewData');

NAMESPACE('chlk.models.attachment', function () {
    "use strict";

    /** @class chlk.models.attachment.FileCabinetViewData*/
    CLASS('FileCabinetViewData', EXTENDS(chlk.models.common.BaseAttachViewData), [

        chlk.models.common.PaginatedList, 'attachments',
        chlk.models.attachment.SortAttachmentType, 'sortType',
        String, 'filter',

        [[chlk.models.common.AttachOptionsViewData, chlk.models.common.PaginatedList, chlk.models.attachment.SortAttachmentType, String]],
        function $(options_, attachments_, sortType_, filter_){
            BASE(options_);
            if(attachments_)
                this.setAttachments(attachments_);
            if(sortType_)
                this.setSortType(sortType_);
            if(filter_)
                this.setFilter(filter_);
        }
    ]);
});
