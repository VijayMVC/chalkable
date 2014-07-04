REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.announcement.Announcement');


NAMESPACE('chlk.templates.announcement', function () {

    /** @class chlk.templates.announcement.StudentAnnouncementsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AnnouncementGradingTopPart.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.StudentAnnouncements)],
        'StudentAnnouncementsTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            Number, 'announcementTypeId',

            [ria.templates.ModelPropertyBind],
            chlk.models.school.SchoolOption, 'schoolOptions',

            [ria.templates.ModelPropertyBind],
            String, 'announcementTitle',

            [ria.templates.ModelPropertyBind],
            Number, 'classAvg',

            [ria.templates.ModelPropertyBind],
            String, 'className',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.CourseId, 'courseId',

            [ria.templates.ModelPropertyBind],
            Number, 'gradedStudentCount',

            [ria.templates.ModelPropertyBind],
            Number, 'gradingStyle',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.announcement.StudentAnnouncement), 'items',

            [ria.templates.ModelPropertyBind],
            chlk.models.grading.Mapping, 'mapping',

            [ria.templates.ModelPropertyBind],
            Number, 'selectedIndex',

            [ria.templates.ModelPropertyBind],
            Boolean, 'showToStudents',

            [ria.templates.ModelPropertyBind],
            chlk.models.announcement.StudentAnnouncement, 'currentItem',

            chlk.models.id.AnnouncementId, 'announcementId',

            Number, 'maxScore',

            Boolean, 'gradable',

            Number, function getAutoGradeCount(){
                return (this.getItems() || []).filter(function(item){
                    return item.getState() == 0;
                }).length;
            },

            String, function getGradedWidth(){
                var res = Math.ceil(100 * this.getGradedStudentCount() / this.getItems().length) + '%';
                return res;
            },

            Object, function getGrade(value){
                var gradingMapping = this.getMapping();
                var gradingStyle = this.getGradingStyle();
                return GradingStyler.getLetterByGrade(value, gradingMapping, gradingStyle)
            }
        ])
});