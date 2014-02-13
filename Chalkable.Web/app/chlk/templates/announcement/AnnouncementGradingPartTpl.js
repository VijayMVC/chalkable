REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.announcement.Announcement');
REQUIRE('chlk.templates.announcement.StudentAnnouncementsTpl');


NAMESPACE('chlk.templates.announcement', function () {

    /** @class chlk.templates.announcement.AnnouncementGradingPartTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AnnouncementGradingPart.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.StudentAnnouncements)],
        'AnnouncementGradingPartTpl', EXTENDS(chlk.templates.announcement.StudentAnnouncementsTpl), [
            Boolean, 'readonly',

            ArrayOf(String), 'autoGradeApps',

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
            }
        ])
});