REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.announcement.FeedAnnouncementViewData');
REQUIRE('chlk.converters.dateTime.DateTimeTextConverter');

NAMESPACE('chlk.templates.announcement', function () {

    /** @class chlk.templates.announcement.AnnouncementQnAs*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AnnouncementQnAs.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.FeedAnnouncementViewData)],
        'AnnouncementQnAs', EXTENDS(chlk.templates.ChlkTemplate), [
            Boolean, 'moreClicked',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AnnouncementId, 'id',

            [ria.templates.ModelPropertyBind],
            Boolean, 'annOwner',

            [ria.templates.ModelPropertyBind],
            chlk.models.people.User, 'owner',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.announcement.AnnouncementQnA), 'announcementQnAs',

            [ria.templates.ModelPropertyBind],
            chlk.models.classes.Class, 'clazz'

        ])
});
