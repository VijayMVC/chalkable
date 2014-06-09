REQUIRE('chlk.models.announcement.Announcement');

NAMESPACE('chlk.templates.announcement', function () {

    /** @class chlk.templates.announcement.AnnouncementForStudentAttachments*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AnnouncementStudentAttachments.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.Announcement)],
        'AnnouncementForStudentAttachments', EXTENDS(chlk.templates.announcement.Announcement), [])
});