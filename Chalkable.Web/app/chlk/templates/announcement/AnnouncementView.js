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
            },

            Number, function getStudentAnnGradingStyle(){return this.getStudentAnnouncements().getGradingStyle();},
            chlk.models.grading.Mapping, function getMapping(){return this.getStudentAnnouncements().getMapping()},

            String, function getGradeLetter(){
                return GradingStyler.getLetterByGrade(this.getGrade(), this.getMapping(), this.getStudentAnnGradingStyle());
            },
            Number, function getGradeNumber(){
                return GradingStyler.getGradeNumberValue(this.getGrade(), this.getMapping(), this.getStudentAnnGradingStyle());
            }
        ])
});