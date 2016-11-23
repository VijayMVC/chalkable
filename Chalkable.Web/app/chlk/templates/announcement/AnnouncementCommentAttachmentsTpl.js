REQUIRE('chlk.templates.announcement.AnnouncementCommentTpl');

NAMESPACE('chlk.templates.announcement', function () {

    /** @class chlk.templates.announcement.AnnouncementCommentAttachmentsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AnnouncementCommentAttachments.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.AnnouncementComment)],
        'AnnouncementCommentAttachmentsTpl', EXTENDS(chlk.templates.announcement.AnnouncementCommentTpl), [])
});