REQUIRE('chlk.templates.PaginatedList');
REQUIRE('chlk.models.announcement.Announcement');

NAMESPACE('chlk.templates.announcement', function () {

    /** @class chlk.templates.announcement.Announcement*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AnnouncementForm.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.Announcement)],
        'AnnouncementForm', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.announcement.AnnouncementId, 'id'
        ])
});