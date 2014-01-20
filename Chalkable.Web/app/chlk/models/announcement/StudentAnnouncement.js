REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.attachment.Attachment');
REQUIRE('chlk.models.announcement.ShortStudentAnnouncementViewData');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.StudentAnnouncement*/
    CLASS(
        'StudentAnnouncement', EXTENDS(chlk.models.announcement.ShortStudentAnnouncementViewData), [

            ArrayOf(chlk.models.attachment.Attachment), 'attachments',

            [ria.serialize.SerializeProperty('studentinfo')],
            chlk.models.people.User, 'studentInfo',

            chlk.models.people.User, 'owner'
        ]);
});
