REQUIRE('chlk.templates.ChlkTemplate');

NAMESPACE('chlk.templates.announcement', function () {

    /** @class chlk.templates.announcement.AnnouncementForStudentAttachments*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AnnouncementStudentAttachments.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.FeedAnnouncementViewData)],
        'AnnouncementForStudentAttachments', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.id.AnnouncementId, 'id',

            [ria.templates.ModelPropertyBind],
            chlk.models.announcement.AnnouncementTypeEnum, 'type',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.attachment.AnnouncementAttachment), 'announcementAttachments',

            [ria.templates.ModelPropertyBind],
            chlk.models.announcement.ClassAnnouncementViewData, 'classAnnouncementData',

            [ria.templates.ModelPropertyBind],
            chlk.models.announcement.StudentAnnouncements, 'studentAnnouncements'
        ])
});