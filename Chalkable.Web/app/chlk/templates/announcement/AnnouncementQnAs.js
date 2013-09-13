REQUIRE('chlk.templates.announcement.Announcement');
REQUIRE('chlk.models.announcement.Announcement');

NAMESPACE('chlk.templates.announcement', function () {

    /** @class chlk.templates.announcement.AnnouncementQnAs*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AnnouncementQnAs.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.Announcement)],
        'AnnouncementQnAs', EXTENDS(chlk.templates.announcement.Announcement), [])
});
