REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.attachment.Attachment');
REQUIRE('chlk.models.standard.Standard');
REQUIRE('chlk.models.announcement.ShortStudentAnnouncementViewData');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.announcement.StudentAnnouncement*/
    CLASS(
        UNSAFE, FINAL, 'StudentAnnouncement', EXTENDS(chlk.models.announcement.ShortStudentAnnouncementViewData), IMPLEMENTS(ria.serialize.IDeserializable), [

            OVERRIDE, VOID, function deserialize(raw){
                BASE(raw);
                this.attachments = SJX.fromArrayOfDeserializables(raw.attachments, chlk.models.attachment.Attachment);
                this.studentInfo = SJX.fromDeserializable(raw.studentinfo, chlk.models.people.User);
                this.owner = SJX.fromDeserializable(raw.owner, chlk.models.people.User);
                this.standards = SJX.fromArrayOfDeserializables(raw.standards, chlk.models.standard.Standard);
                this.standardsIds = SJX.fromValue(raw.standardsids, String);
                this.standardsGrades = SJX.fromValue(raw.standardsgrades, String);
            },

            ArrayOf(chlk.models.attachment.Attachment), 'attachments',
            chlk.models.people.User, 'studentInfo',
            chlk.models.people.User, 'owner',
            ArrayOf(chlk.models.standard.Standard), 'standards',

            Object, function getGrade(value){
                return value;//GradingStyler.getLetterByGrade(value, this.getGradingMapping(), this.getGradingStyle())
            },

            Object, function isGradeDisabled(){
                return this.isDropped() || this.isExempt();
            },

            String, 'standardsGrades',
            String, function getStandardsGrades(){
                var res = [];
                this.getStandards().forEach(function(item){
                    res.push(item.getGrade());
                });
                return res.join(',');
            },

            String, 'standardsIds',
            String, function getStandardsIds(){
                var res = [];
                this.getStandards().forEach(function(item){
                    res.push(item.getStandardId().valueOf());
                });
                return res.join(',');
            }
        ]);
});
