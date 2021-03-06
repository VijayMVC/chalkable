REQUIRE('chlk.templates.announcement.ApplicationsAndAttachments');
REQUIRE('chlk.models.announcement.Announcement');
REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.id.StudentAnnouncementId');
REQUIRE('chlk.models.id.MarkingPeriodId');


NAMESPACE('chlk.templates.announcement', function () {

    /** @class chlk.templates.announcement.Announcement*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AnnouncementAppAttachments.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.Announcement)],
        'Announcement', EXTENDS(chlk.templates.announcement.ApplicationsAndAttachments), [

            [ria.templates.ModelPropertyBind],
            chlk.models.announcement.AnnouncementTypeEnum, 'type',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AnnouncementId, 'id',

            [ria.templates.ModelPropertyBind],
            String, 'className',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'created',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.announcement.AdminAnnouncementRecipient), 'recipients',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.attachment.AnnouncementAttachment), 'announcementAttachments',

            [ria.templates.ModelPropertyBind],
            Number, 'announcementTypeId', // make enum

            [ria.templates.ModelPropertyBind],
            Number, 'chalkableAnnouncementType',

            [ria.templates.ModelPropertyBind],
            Boolean, 'ableToGrade',

            [ria.templates.ModelPropertyBind],
            Boolean, 'adminAnnouncement',

            [ria.templates.ModelPropertyBind],
            Boolean, 'standartAnnouncement',

            [ria.templates.ModelPropertyBind],
            String, 'announcementTypeName',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.apps.AppAttachment), 'applications',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.apps.AppAttachment), 'gradeViewApps',

            [ria.templates.ModelPropertyBind],
            String, 'applicationName',

            [ria.templates.ModelPropertyBind],
            Number, 'applicationsCount',

            [ria.templates.ModelPropertyBind],
            String, 'attachments',

            [ria.templates.ModelPropertyBind],
            String, 'applicationsIds',

            [ria.templates.ModelPropertyBind],
            Number, 'attachmentsCount',

            [ria.templates.ModelPropertyBind],
            Number, 'attachmentNames',

            [ria.templates.ModelPropertyBind],
            Number, 'attachmentsSummary',

            [ria.templates.ModelPropertyBind],
            Array, 'autoGradeApps',

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
            chlk.models.id.ClassId, 'classId',

            [ria.templates.ModelPropertyBind],
            Boolean, 'dropped',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'expiresDate',

            [ria.templates.ModelPropertyBind],
            String, 'expiresDateColor',

            [ria.templates.ModelPropertyBind],
            String, 'expiresDateText',

            [ria.templates.ModelPropertyBind],
            Boolean, 'gradable',

            [ria.templates.ModelPropertyBind],
            Number, 'grade',

            [ria.templates.ModelPropertyBind],
            Number, 'gradesSummary',

            [ria.templates.ModelPropertyBind],
            Number, 'gradingStudentsCount',


            [ria.templates.ModelPropertyBind],
            Boolean, 'annOwner',

            [ria.templates.ModelPropertyBind],
            Boolean, 'ableEdit',

            [ria.templates.ModelPropertyBind],
            Number, 'nonGradingStudentsCount',

            [ria.templates.ModelPropertyBind],
            Number, 'order',

            [ria.templates.ModelPropertyBind],
            chlk.models.people.User, 'owner',

            [ria.templates.ModelPropertyBind],
            Number, 'ownerAttachmentsCount',

            [ria.templates.ModelPropertyBind],
            Number, 'qnaCount',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassId, 'recipientId',

            [ria.templates.ModelPropertyBind],
            String, 'schoolPersonGender',

            [ria.templates.ModelPropertyBind],
            String, 'schoolPersonName',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.SchoolPersonId, 'personId',

            [ria.templates.ModelPropertyBind],
            String, 'shortContent',

            [ria.templates.ModelPropertyBind],
            Boolean, 'showGradingIcon',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.standard.Standard), 'standards',

            [ria.templates.ModelPropertyBind],
            Boolean, 'complete',

            [ria.templates.ModelPropertyBind],
            Number, 'state',

            [ria.templates.ModelPropertyBind],
            Number, 'stateTyped',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.StudentAnnouncementId, 'studentAnnouncementId',

            [ria.templates.ModelPropertyBind],
            chlk.models.announcement.StudentAnnouncements, 'studentAnnouncements',

            [ria.templates.ModelPropertyBind],
            Number, 'studentsCount',

            [ria.templates.ModelPropertyBind],
            Number, 'studentsWithAttachmentsCount',

            [ria.templates.ModelPropertyBind],
            Number, 'studentsWithoutAttachmentsCount',

            [ria.templates.ModelPropertyBind],
            Number, 'systemType',

            [ria.templates.ModelPropertyBind],
            String, 'title',

            [ria.templates.ModelPropertyBind],
            Boolean, 'wasAnnouncementTypeGraded',

            [ria.templates.ModelPropertyBind],
            String, 'submitType',

            [ria.templates.ModelPropertyBind],
            Boolean, 'needButtons',

            [ria.templates.ModelPropertyBind],
            Boolean, 'needDeleteButton',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.announcement.AnnouncementQnA), 'announcementQnAs',

            [ria.templates.ModelPropertyBind],
            String, 'annRecipients',

            [ria.templates.ModelPropertyBind],
            Number, 'maxScore',

            [ria.templates.ModelPropertyBind],
            Boolean, 'hiddenFromStudents',

            [ria.templates.ModelPropertyBind],
            Boolean, 'ableDropStudentScore',

            [ria.templates.ModelPropertyBind],
            Boolean, 'ableToExempt',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AppId, 'assessmentApplicationId',

            [ria.templates.ModelPropertyBind],
            Boolean, 'canAddStandard',

            String, function getStandardsUrlComponents() {
                return (this.standards || []).map(function (c, index) { return c.getUrlComponents(index); }).join('&');
            }
        ]);
});