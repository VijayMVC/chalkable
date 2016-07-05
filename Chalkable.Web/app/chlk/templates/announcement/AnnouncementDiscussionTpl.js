REQUIRE('chlk.templates.announcement.AnnouncementView');

NAMESPACE('chlk.templates.announcement', function () {

    /** @class chlk.templates.announcement.AnnouncementDiscussionTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AnnouncementDiscussion.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.AnnouncementView)],
        'AnnouncementDiscussionTpl', EXTENDS(chlk.templates.announcement.AnnouncementView), [])
});