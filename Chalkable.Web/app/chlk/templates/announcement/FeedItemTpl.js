REQUIRE('chlk.models.announcement.Announcement');
REQUIRE('chlk.templates.announcement.Announcement');

NAMESPACE('chlk.templates.announcement', function () {

    /** @class chlk.templates.announcement.FeedItemTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/FeedItem.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.Announcement)],
        'FeedItemTpl', EXTENDS(chlk.templates.announcement.Announcement), []);
});