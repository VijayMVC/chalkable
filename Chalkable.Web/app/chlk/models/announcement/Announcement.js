REQUIRE('chlk.models.announcement.AnnouncementType');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.AnnouncementId*/
    IDENTIFIER('AnnouncementId');

    /** @class chlk.models.announcement.AnnouncementTypeId*/
    IDENTIFIER('AnnouncementTypeId');

    /** @class chlk.models.announcement.AnnouncementRecipientId*/
    IDENTIFIER('AnnouncementRecipientId');

    /** @class chlk.models.announcement.ClassId*/
    IDENTIFIER('ClassId');

    /** @class chlk.models.announcement.SchoolPersonId*/
    IDENTIFIER('SchoolPersonId');

    /** @class chlk.models.announcement.StudentAnnouncementId*/
    IDENTIFIER('StudentAnnouncementId');

    /** @class chlk.models.announcement.Announcement*/
    CLASS(
        'Announcement', [
            chlk.models.announcement.AnnouncementId, 'id',
            chlk.models.announcement.AnnouncementTypeId, 'announcementtypeid',
            String, 'announcementtypename',
            String, 'applicationname',
            Number, 'applicationscount',
            Number, 'attachmentscount',
            Number, 'attachmentsummary',
            Number, 'avg',
            Number, 'avgnumeric',
            Object, 'class',
            String, 'comment',
            String, 'content',
            Date, 'created',
            Number, 'dropped',
            String, 'expiresdate',
            Boolean, 'gradable',
            Number, 'grade',
            Number, 'gradesummary',
            Number, 'gradingstudentscount',
            Number, 'gradingstyle',
            Boolean, 'isowner',
            Number, 'nongradingstudentscount',
            Number, 'order',
            Number, 'ownerattachmentscount',
            Number, 'qnacount',
            chlk.models.announcement.AnnouncementRecipientId, 'recipientid',
            String, 'schoolpersongender',
            String, 'schoolpersonname',
            chlk.models.announcement.SchoolPersonId, 'schoolpersonref',
            Number, 'shortcontent',
            Boolean, 'showgradingicon',
            Boolean, 'starred',
            Number, 'state',
            Number, 'statetyped',
            chlk.models.announcement.StudentAnnouncementId, 'studentannouncementid',
            Number, 'studentscount',
            Number, 'studentscountwithattachments',
            Number, 'studentscountwithoutattachments',
            String, 'subject',
            Number, 'systemtype',
            String, 'title',
            Boolean, 'wasannouncementtypegraded'
        ]);
});
