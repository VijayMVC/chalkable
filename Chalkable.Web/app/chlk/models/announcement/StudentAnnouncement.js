REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.attachment.Attachment');
REQUIRE('chlk.models.standard.Standard');
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

            Object, function isGradeDisabled(){
                return this.isDropped() || this.isExempt();
            },

            ArrayOf(chlk.models.standard.Standard), 'standards',

            String, function getStandardsGrades(){
                var res = [];
                this.getStandards().forEach(function(item){
                    res.push(item.getGrade());
                });
                return res.join(',');
            },

            String, function getStandardsIds(){
                var res = [];
                this.getStandards().forEach(function(item){
                    res.push(item.getStandardId().valueOf());
                });
                return res.join(',');
            },

            String, 'standardIds',

            String, 'standardGrades'
        ]);
});
