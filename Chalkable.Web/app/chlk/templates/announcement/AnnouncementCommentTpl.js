REQUIRE('chlk.models.announcement.AnnouncementComment');
REQUIRE('chlk.templates.ChlkTemplate');

NAMESPACE('chlk.templates.announcement', function () {

    /** @class chlk.templates.announcement.AnnouncementCommentTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AnnouncementComment.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.AnnouncementComment)],
        'AnnouncementCommentTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AnnouncementCommentId, 'id',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AnnouncementId, 'announcementId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AnnouncementCommentId, 'parentCommentId',

            [ria.templates.ModelPropertyBind],
            chlk.models.attachment.Attachment, 'attachment',

            [ria.templates.ModelPropertyBind],
            chlk.models.people.User, 'owner',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'postedDate',

            [ria.templates.ModelPropertyBind],
            String, 'text',

            [ria.templates.ModelPropertyBind],
            Boolean, 'hidden',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AttachmentId, 'attachmentId',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.announcement.AnnouncementComment), 'subComments'
        ])
});