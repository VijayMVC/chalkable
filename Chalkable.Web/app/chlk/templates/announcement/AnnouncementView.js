REQUIRE('chlk.templates.announcement.Announcement');
REQUIRE('chlk.models.announcement.Announcement');
REQUIRE('chlk.templates.announcement.AnnouncementQnAs');


NAMESPACE('chlk.templates.announcement', function () {

    /** @class chlk.templates.announcement.AnnouncementView*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AnnouncementView.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.Announcement)],
        'AnnouncementView', EXTENDS(chlk.templates.announcement.Announcement), [
            Number, function getAutoGradeCount(){
                return this.getStudentAnnouncements().getItems().filter(function(item){
                    return item.getState() == 0;
                }).length;
            }
        ])
});