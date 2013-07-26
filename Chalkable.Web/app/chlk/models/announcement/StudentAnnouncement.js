REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.attachment.Attachment');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.StudentAnnouncementId*/
    IDENTIFIER('StudentAnnouncementId');

    /** @class chlk.models.announcement.StudentAnnouncement*/
    CLASS(
        'StudentAnnouncement', [
            [ria.serialize.SerializeProperty('announcementid')],
            chlk.models.announcement.AnnouncementId, 'announcementId',

            ArrayOf(chlk.models.attachment.Attachment), 'attachments',

            String, 'comment',

            Boolean, 'dropped',

            [ria.serialize.SerializeProperty('gradevalue')],
            Number, 'gradeValue',

            chlk.models.announcement.StudentAnnouncementId, 'id',

            Number, 'state',

            [ria.serialize.SerializeProperty('studentinfo')],
            chlk.models.people.User, 'studentInfo'
        ]);
});
