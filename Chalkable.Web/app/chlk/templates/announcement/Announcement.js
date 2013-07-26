REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.announcement.Announcement');

NAMESPACE('chlk.templates.announcement', function () {

    /** @class chlk.templates.announcement.Announcement*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AnnouncementFormContent.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.Announcement)],
        'Announcement', EXTENDS(chlk.templates.JadeTemplate), [

            [ria.templates.ModelPropertyBind],
            chlk.models.announcement.AnnouncementId, 'id',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'created',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.attachment.Attachment), 'announcementAttachments',

            [ria.templates.ModelPropertyBind],
            Number, 'announcementTypeId', // make enum

            [ria.templates.ModelPropertyBind],
            String, 'announcementTypeName',

            [ria.templates.ModelPropertyBind],
            String, 'applicationName',

            [ria.templates.ModelPropertyBind],
            Number, 'applicationsCount',

            [ria.templates.ModelPropertyBind],
            String, 'attachments',

            [ria.templates.ModelPropertyBind],
            Number, 'attachmentsCount',

            [ria.templates.ModelPropertyBind],
            Number, 'attachmentsSummary',

            [ria.templates.ModelPropertyBind],
            Number, 'avg',

            [ria.templates.ModelPropertyBind],
            Number ,'avgNumeric',

            [ria.templates.ModelPropertyBind],
            Object, 'clazz',

            [ria.templates.ModelPropertyBind],
            String, 'comment',

            [ria.templates.ModelPropertyBind],
            String, 'content',

            [ria.templates.ModelPropertyBind],
            chlk.models.class.ClassId, 'classId',

            [ria.templates.ModelPropertyBind],
            Number, 'dropped',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'expiresDate',

            [ria.templates.ModelPropertyBind],
            Boolean, 'gradable',

            [ria.templates.ModelPropertyBind],
            Number, 'grade',

            [ria.templates.ModelPropertyBind],
            Number, 'gradesSummary',

            [ria.templates.ModelPropertyBind],
            Number, 'gradingStudentsCount',

            [ria.templates.ModelPropertyBind],
            Number, 'gradingStyle',

            [ria.templates.ModelPropertyBind],
            Boolean, 'isOwner',

            [ria.templates.ModelPropertyBind],
            Number, 'nonGradingStudentsCount',

            [ria.templates.ModelPropertyBind],
            Number, 'order',

            [ria.templates.ModelPropertyBind],
            Number, 'ownerAttachmentsCount',

            [ria.templates.ModelPropertyBind],
            Number, 'qnaCount',

            [ria.templates.ModelPropertyBind],
            chlk.models.class.ClassId, 'recipientId',

            [ria.templates.ModelPropertyBind],
            String, 'schoolPersonGender',

            [ria.templates.ModelPropertyBind],
            String, 'schoolPersonName',

            [ria.templates.ModelPropertyBind],
            chlk.models.people.SchoolPersonId, 'schoolPersonRef',

            [ria.templates.ModelPropertyBind],
            String, 'shortContent',

            [ria.templates.ModelPropertyBind],
            Boolean, 'showGradingIcon',

            [ria.templates.ModelPropertyBind],
            Boolean, 'starred',

            [ria.templates.ModelPropertyBind],
            Number, 'state',

            [ria.templates.ModelPropertyBind],
            Number, 'stateTyped',

            [ria.templates.ModelPropertyBind],
            chlk.models.announcement.StudentAnnouncementId, 'studentannouncementid',

            [ria.templates.ModelPropertyBind],
            Number, 'studentsCount',

            [ria.templates.ModelPropertyBind],
            Number, 'studentsWithAttachmentsCount',

            [ria.templates.ModelPropertyBind],
            Number, 'studentsWithoutAttachmentsCount',

            [ria.templates.ModelPropertyBind],
            String, 'subject',

            [ria.templates.ModelPropertyBind],
            Number, 'systemType',

            [ria.templates.ModelPropertyBind],
            String, 'title',

            [ria.templates.ModelPropertyBind],
            Boolean, 'wasAnnouncementTypeGraded',

            [ria.templates.ModelPropertyBind],
            String, 'submitType'
        ])
});