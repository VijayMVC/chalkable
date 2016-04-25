REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.id.AnnouncementAssignedAttributeId');
REQUIRE('chlk.models.announcement.BaseAnnouncementViewData');
REQUIRE('chlk.models.attachment.Attachment');

NAMESPACE('chlk.models.attachment', function () {
    "use strict";

    /** @class chlk.models.attachment.FileCabinetPostData*/
    CLASS(
        'FileCabinetPostData', [

            [ria.serialize.SerializeProperty('announcementid')],
            chlk.models.id.AnnouncementId, 'announcementId',

            [ria.serialize.SerializeProperty('announcementtype')],
            chlk.models.announcement.AnnouncementTypeEnum, 'announcementType',

            [ria.serialize.SerializeProperty('assignedattributeid')],
            chlk.models.id.AnnouncementAssignedAttributeId, 'assignedAttributeId',

            [ria.serialize.SerializeProperty('sorttype')],
            chlk.models.attachment.SortAttachmentType, 'sortType',

            Number, 'start',
            Number, 'count',
            String, 'filter'

        ]);
});
