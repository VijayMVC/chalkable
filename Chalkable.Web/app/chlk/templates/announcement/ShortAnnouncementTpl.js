REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.announcement.ShortAnnouncementViewData');

NAMESPACE('chlk.templates.announcement', function () {

    /** @class chlk.templates.announcement.ShortAnnouncementTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AnnouncementAppAttachments.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.ShortAnnouncementViewData)],
        'ShortAnnouncementTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            String, 'announcementTypeName',

            [ria.templates.ModelPropertyBind],
            Number, 'order',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AnnouncementId, 'id',

            [ria.templates.ModelPropertyBind],
            Boolean, 'dropped',

            [ria.templates.ModelPropertyBind],
            chlk.models.announcement.ShortStudentAnnouncementsViewData, 'studentAnnouncements'
        ]);
});