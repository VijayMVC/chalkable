REQUIRE('chlk.templates.Popup');
REQUIRE('chlk.models.grading.ClassPersonGradingItem');

NAMESPACE('chlk.templates.student', function(){
    "use strict";

    /**@class chlk.templates.student.StudentProfileGradingPopUpTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/student/StudentProfileGradingPopUp.jade')],
        [ria.templates.ModelBind(chlk.models.grading.ClassPersonGradingItem)],
        'StudentProfileGradingPopUpTpl', EXTENDS(chlk.templates.Popup),[

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.profile.ShortAnnouncementForProfileViewData), 'announcements',

            Object, function getNormalValue(announcement){
                var studentAnnouncement = announcement.getStudentAnnouncements().getItems()[0];
                var value = studentAnnouncement.getGradeValue();
                if(studentAnnouncement.isDropped() && !value)
                    return Msg.Dropped;
                if(studentAnnouncement.isExempt())
                    return Msg.Exempt;
                return value;
            }
        ]);
});