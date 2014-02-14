REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.attachment.Attachment');
REQUIRE('chlk.models.announcement.ShortStudentAnnouncementViewData');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.StudentAnnouncement*/
    CLASS(
        'StudentAnnouncement', EXTENDS(chlk.models.announcement.ShortStudentAnnouncementViewData), [

            ArrayOf(chlk.models.attachment.Attachment), 'attachments',

            [ria.serialize.SerializeProperty('studentinfo')],
            chlk.models.people.User, 'studentInfo',

            chlk.models.people.User, 'owner',

            Object, function getGrade(value){
                return value;//GradingStyler.getLetterByGrade(value, this.getGradingMapping(), this.getGradingStyle())
            },

            Object, function getNormalValue(){
                var value = this.getGradeValue();
                if(this.isDropped())
                    return Msg.Dropped;
                if(this.isExempt())
                    return Msg.Exempt;
                return (value >= 0) ? value : '';
            },

            Object, function isGradeDisabled(){
                return this.isDropped() || this.isExempt();
            }
        ]);
});
