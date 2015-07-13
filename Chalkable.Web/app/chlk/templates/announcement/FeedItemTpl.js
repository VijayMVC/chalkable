REQUIRE('chlk.models.announcement.FeedAnnouncementViewData');
REQUIRE('chlk.templates.ChlkTemplate');

NAMESPACE('chlk.templates.announcement', function () {

    /** @class chlk.templates.announcement.FeedItemTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/FeedItem.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.FeedAnnouncementViewData)],
        'FeedItemTpl', EXTENDS(chlk.templates.ChlkTemplate), [

        ]);
});