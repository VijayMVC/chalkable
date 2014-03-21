REQUIRE('chlk.templates.announcement.Announcement');
REQUIRE('chlk.models.announcement.Announcement');
REQUIRE('chlk.converters.dateTime.DateTimeTextConverter');

NAMESPACE('chlk.templates.announcement', function () {

    /** @class chlk.templates.announcement.AnnouncementQnAs*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AnnouncementQnAs.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.Announcement)],
        'AnnouncementQnAs', EXTENDS(chlk.templates.announcement.Announcement), [

            OVERRIDE, chlk.models.people.User, function getCurrentUser(){
                return this.getModel().getCurrentUser();
            }
        ])
});
