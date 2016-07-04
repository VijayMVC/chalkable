REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.announcement.ClassAnnouncementViewData');
REQUIRE('chlk.templates.announcement.StudentAnnouncementsTpl');
REQUIRE('chlk.models.announcement.StudentAnnouncementApplicationMeta');


NAMESPACE('chlk.templates.announcement', function () {

    /** @class chlk.templates.announcement.AnnouncementGradingPartTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AnnouncementGradingPart.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.StudentAnnouncements)],
        'AnnouncementGradingPartTpl', EXTENDS(chlk.templates.announcement.StudentAnnouncementsTpl), [
            Boolean, 'readonly',

            Array, 'autoGradeApps',

            ArrayOf(chlk.models.apps.AppAttachment), 'gradeViewApps',

            ArrayOf(chlk.models.apps.AppAttachment), 'applications',

            ArrayOf(chlk.models.announcement.StudentAnnouncementApplicationMeta), 'studentsAnnApplicationMeta',

            chlk.models.people.User, 'owner',

            Boolean, 'ableToExempt',

            ArrayOf(chlk.models.standard.Standard), 'standards',

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

            String, function getStandardsUrlComponents() {
                return (this.standards || []).map(function (c, index) { return c.getUrlComponents(index); }).join('&');
            },

            function getAppGrade(announcementApplicationId, studentId){
                var app = this.getAutoGradeApps().filter(function(item){return item.id == announcementApplicationId})[0];

                if(!app)
                    return null;

                var student = app.students.filter(function(item){return item.id == studentId})[0];

                if(!student)
                    return null;

                return student.grade;
            },

            String, function getStudentAnnApplicationMetaText(announcementApplicationId, studentId){
                var texts = (this.getStudentsAnnApplicationMeta() || []).filter(function(item){
                    return item.getAnnouncementApplicationId().valueOf() == announcementApplicationId.valueOf()
                            && item.getStudentId().valueOf() == studentId.valueOf();
                });

                return texts[0] ? texts[0].getText() : '';
            }
        ])
});