REQUIRE('chlk.templates.announcement.Announcement');
REQUIRE('chlk.models.announcement.AnnouncementView');
REQUIRE('chlk.templates.announcement.AnnouncementQnAs');


NAMESPACE('chlk.templates.announcement', function () {

    /** @class chlk.templates.announcement.AnnouncementView*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AnnouncementView.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.AnnouncementView)],
        'AnnouncementView', EXTENDS(chlk.templates.announcement.Announcement), [
            Number, function getAutoGradeCount(){
                return (this.getStudentAnnouncements().getItems() || []).filter(function(item){
                    return item.getState() == 0;
                }).length;
            },

            [ria.templates.ModelPropertyBind],
            Boolean, 'exempt',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.AlphaGrade), 'alphaGrades',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.AlternateScore), 'alternateScores',

            Number, function getStudentAnnGradingStyle(){return this.getStudentAnnouncements().getGradingStyle();},
            chlk.models.grading.Mapping, function getMapping(){return this.getStudentAnnouncements().getMapping()}
        ])
});