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

            [[Number, Boolean]],
            String, function getTooltipText(maxScore, byNumeric_){
                var res = [];
                if(this.isLate())
                    res.push(Msg.Late);
                if(this.isIncomplete())
                    res.push(Msg.Incomplete);
                if(this.isAbsent())
                    res.push(Msg.Student_marked_absent);
                if(byNumeric_){
                   if(this.getNumericGradeValue() > maxScore)
                        res.push(Msg.Scores_exceeds);
                }
                else{
                    var value = parseInt(this.getGradeValue(), 10);
                    if(value && value > maxScore)
                        res.push(Msg.Scores_exceeds);
                }
                if(!res.length)
                    return '';
                return res.join('<hr>');
            },

            [[Number, Boolean]],
            String, function getAlertClass(maxScore, byNumeric_){
                var classes = [];
                if(byNumeric_){
                    if(this.getNumericGradeValue() > maxScore)
                        classes.push(Msg.Error.toLowerCase());
                }
                else{
                    var value = parseInt(this.getGradeValue(), 10);
                    if(value && value > maxScore)
                        classes.push(Msg.Error.toLowerCase());
                }
                if(this.isLate())
                    classes.push(Msg.Late.toLowerCase());
                if(this.isIncomplete())
                    classes.push(Msg.Incomplete.toLowerCase());
                if(this.isAbsent())
                    classes.push(Msg.Absent.toLowerCase());
                if(classes.length > 1)
                    return Msg.Multiple.toLowerCase();
                if(classes.length == 1)
                    return classes[0];
                return '';
            },

            Object, function isGradeDisabled(){
                return this.isDropped() || this.isExempt();
            }
        ]);
});
