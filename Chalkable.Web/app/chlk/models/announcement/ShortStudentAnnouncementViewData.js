
REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.id.StudentAnnouncementId');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.ShortStudentAnnouncementViewData*/
    CLASS(
        'ShortStudentAnnouncementViewData', [
            [ria.serialize.SerializeProperty('announcementid')],
            chlk.models.id.AnnouncementId, 'announcementId',

            chlk.models.id.SchoolPersonId, 'studentId',

            String, 'comment',

            Boolean, 'dropped',

            ArrayOf(String), 'alerts',

            [ria.serialize.SerializeProperty('gradevalue')],
            String, 'gradeValue',

            [ria.serialize.SerializeProperty('numericgradevalue')],
            Number, 'numericGradeValue',

            chlk.models.id.StudentAnnouncementId, 'id',

            Number, 'state',

            [ria.serialize.SerializeProperty('islate')],
            Boolean, 'late',

            [ria.serialize.SerializeProperty('isabsent')],
            Boolean, 'absent',

            [ria.serialize.SerializeProperty('isincomplete')],
            Boolean, 'incomplete',

            [ria.serialize.SerializeProperty('isexempt')],
            Boolean, 'exempt',

            [ria.serialize.SerializeProperty('ispassed')],
            Boolean, 'passed',

            [ria.serialize.SerializeProperty('iscomplete')],
            Boolean, 'complete',

            [[Number, Boolean]],
            String, function getAlertClass(maxScore, byNumeric_){
                var classes = [];
                if(byNumeric_){
                    if(this.getNumericGradeValue() > maxScore)
                        classes.push(Msg.Error.toLowerCase());
                }
                else{
                    var value = parseFloat(this.getGradeValue());
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
                    var value = parseFloat(this.getGradeValue());
                    if(value && value > maxScore)
                        res.push(Msg.Scores_exceeds);
                }
                if(!res.length)
                    return '';
                return res.join('<hr>');
            }
        ]);
});
