REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.announcement.StudentAnnouncement');

NAMESPACE('chlk.templates.announcement', function(){

    /**@class chlk.templates.announcement.AnnouncementViewStandardsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AnnouncementViewStandards.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.StudentAnnouncement)],
        'AnnouncementViewStandardsTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.standard.Standard), 'standards'
    ]);
});