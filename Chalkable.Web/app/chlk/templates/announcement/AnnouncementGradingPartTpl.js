REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.announcement.Announcement');


NAMESPACE('chlk.templates.announcement', function () {

    /** @class chlk.templates.announcement.AnnouncementGradingPartTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AnnouncementGradingPart.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.StudentAnnouncements)],
        'AnnouncementGradingPartTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            Number, 'announcementTypeId',

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

            ArrayOf(String), 'autoGradeApps',

            Boolean, 'readonly',

            //Boolean, 'noAnnouncement',

            chlk.models.id.AnnouncementId, 'announcementId',

            ArrayOf(chlk.models.apps.AppAttachment), 'gradeViewApps',

            ArrayOf(chlk.models.apps.AppAttachment), 'applications',

            chlk.models.people.User, 'owner',

            ArrayOf(chlk.models.announcement.StudentAnnouncement), function getSortedStudentAnnouncements(){
                var studentAnnouncement = this.getItems().slice(), res=[];
                studentAnnouncement.forEach(function(item){
                    if(item.getState() == 0)
                        res.unshift(item);
                    else
                        res.push(item);
                });
                return res;
            },

            Number, function getAutoGradeCount(){
                return this.getItems().filter(function(item){
                    return item.getState() == 0;
                }).length;
            },

            String, function getGradedWidth(){
                var res = Math.ceil(100 * this.getGradedStudentCount() / this.getItems().length) + '%';
                console.info(this.getGradedStudentCount(), this.getItems().length, res);
                return res;
            }
        ])
});