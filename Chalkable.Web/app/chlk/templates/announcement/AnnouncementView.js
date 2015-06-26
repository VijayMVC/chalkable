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

            Boolean, function isStudentGraded(){
                var grade = this.getGrade();
                return this.isDropped() || this.isExempt() || grade >= 0;
            },

            String, function displayStudentGradeValue(){
                if(this.isDropped()) return Msg.Dropped;
                if(this.isExempt()) return Msg.Exempt;
                var grade = this.getGrade();
                if(grade || grade == 0) return grade.toString();
                return '';
            }
        ])
});