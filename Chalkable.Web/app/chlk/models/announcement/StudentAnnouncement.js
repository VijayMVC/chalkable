REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.attachment.Attachment');
REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.id.StudentAnnouncementId');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.StudentAnnouncement*/
    CLASS(
        'StudentAnnouncement', [
            [ria.serialize.SerializeProperty('announcementid')],
            chlk.models.id.AnnouncementId, 'announcementId',

            ArrayOf(chlk.models.attachment.Attachment), 'attachments',

            String, 'comment',

            Boolean, 'dropped',

            ArrayOf(String), 'alerts',

            [ria.serialize.SerializeProperty('gradevalue')],
            Number, 'gradeValue',

            chlk.models.id.StudentAnnouncementId, 'id',

            Number, 'state',

            [ria.serialize.SerializeProperty('studentinfo')],
            chlk.models.people.User, 'studentInfo',

            [ria.serialize.SerializeProperty('islate')],
            Boolean, 'late',

            [ria.serialize.SerializeProperty('isabsent')],
            Boolean, 'absent',

            [ria.serialize.SerializeProperty('isincomplete')],
            Boolean, 'incomplete',

            [ria.serialize.SerializeProperty('isexempt')],
            Boolean, 'exempt',

            chlk.models.people.User, 'owner'
        ]);
});
