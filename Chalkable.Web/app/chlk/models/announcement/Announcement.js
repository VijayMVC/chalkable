REQUIRE('chlk.models.announcement.AnnouncementType');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.AnnouncementId*/
    IDENTIFIER('AnnouncementId');

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
            chlk.models.common.ChlkDate, 'created',

            [ria.serialize.SerializeProperty('announcementtypeid')],
            Number, 'announcementTypeId', // make enum

            [ria.serialize.SerializeProperty('announcementtypename')],
            String, 'announcementTypeName',

            [ria.serialize.SerializeProperty('applicationname')],
            String, 'applicationName',

            [ria.serialize.SerializeProperty('applicationscount')],
            Number, 'applicationsCount',

            [ria.serialize.SerializeProperty('attachmentscount')],
            Number, 'attachmentsCount',

            [ria.serialize.SerializeProperty('attachmentsummary')],
            Number, 'attachmentsSummary',

            Number, 'avg',

            [ria.serialize.SerializeProperty('avgnumeric')],
            Number ,'avgNumeric',

            Object, 'class',
            String, 'comment',
            String, 'content',
            Date, 'created',
            Number, 'dropped',

            [ria.serialize.SerializeProperty('expiresdate')],
            chlk.model.common.ChlkDate, 'expiresDate',

            Boolean, 'gradable',
            Number, 'grade',

            [ria.serialize.SerializeProperty('gradesummary')],
            Number, 'gradesSummary',

            [ria.serialize.SerializeProperty('gradingstudentscount')],
            Number, 'gradingStudentsCount',

            [ria.serialize.SerializeProperty('gradingstyle')],
            Number, 'gradingStyle',

            [ria.serialize.SerializeProperty('isowner')],
            Boolean, 'isOwner',

            [ria.serialize.SerializeProperty('nongradingstudentscount')],
            Number, 'nonGradingStudentsCount',

            Number, 'order',

            [ria.serialize.SerializeProperty('ownerattachmentscount')],
            Number, 'ownerAttachmentsCount',

            [ria.serialize.SerializeProperty('qnacount')],
            Number, 'qnaCount',

            [ria.serialize.SerializeProperty('recipientid')],
            chlk.models.announcement.AnnouncementRecipientId, 'recipientId',

            [ria.serialize.SerializeProperty('schoolpersongender')],
            String, 'schoolPersonGender',

            [ria.serialize.SerializeProperty('schoolpersonname')],
            String, 'schoolPersonName',

            [ria.serialize.SerializeProperty('schoolpersonref')],
            chlk.models.announcement.SchoolPersonId, 'schoolPersonRef',

            [ria.serialize.SerializeProperty('shortcontent')],
            String, 'shortContent',

            [ria.serialize.SerializeProperty('showgradingicon')],
            Boolean, 'showGradingIcon',

            Boolean, 'starred',
            Number, 'state',

            [ria.serialize.SerializeProperty('statetyped')],
            Number, 'stateTyped',

            [ria.serialize.SerializeProperty('studentannouncementid')],
            chlk.models.announcement.StudentAnnouncementId, 'studentannouncementid',

            [ria.serialize.SerializeProperty('studentscount')],
            Number, 'studentsCount',

            [ria.serialize.SerializeProperty('studentscountwithattachments')],
            Number, 'studentsWithAttachmentsCount'

            [ria.serialize.SerializeProperty('studentscountwithoutattachments')],
            Number, 'studentsWithoutAttachmentsCount',

            String, 'subject',
            [ria.serialize.SerializeProperty('systemtype')],

            Number, 'systemType',
            String, 'title',

            [ria.serialize.SerializeProperty('wasannouncementtypegraded')],
            Boolean, 'wasAnnouncementTypeGraded'
        ]);
});
