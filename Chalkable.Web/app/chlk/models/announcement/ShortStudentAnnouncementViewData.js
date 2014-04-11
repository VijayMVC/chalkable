
REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.id.StudentAnnouncementId');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.ShortStudentAnnouncementViewData*/
    CLASS(
        'ShortStudentAnnouncementViewData', [
            [ria.serialize.SerializeProperty('announcementid')],
            chlk.models.id.AnnouncementId, 'announcementId',

            [ria.serialize.SerializeProperty('studentid')],
            chlk.models.id.SchoolPersonId, 'studentId',

            String, 'comment',

            Boolean, 'dropped',

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

            [[chlk.models.id.StudentAnnouncementId, chlk.models.id.AnnouncementId, chlk.models.id.SchoolPersonId,
                Boolean, Boolean, Boolean, Boolean, Boolean, String, String]],
            function $(id_, announcementId_, studentId_, dropped_, late_, exempt_, absent_, incomplete_, comment_, gradeValue_){
                BASE();
                if(id_)
                    this.setId(id_);
                if(announcementId_)
                    this.setAnnouncementId(announcementId_);
                if(studentId_)
                    this.setStudentId(studentId_);
                if(dropped_)
                    this.setDropped(dropped_);
                if(late_)
                    this.setLate(late_);
                if(exempt_)
                    this.setExempt(exempt_);
                if(absent_)
                    this.setAbsent(absent_);
                if(incomplete_)
                    this.setIncomplete(incomplete_);
                if(comment_)
                    this.setComment(comment_);
                if(gradeValue_)
                    this.setGradeValue(gradeValue_);
            },

            [[Number, Boolean]],
            String, function getAlertClass(maxScore, byNumeric_){
                var classes = [];
                if(maxScore || maxScore == 0){
                    if(byNumeric_){
                        if(this.getNumericGradeValue() > maxScore)
                            classes.push(Msg.Error.toLowerCase());
                    }
                    else{
                        var value = parseFloat(this.getGradeValue());
                        if(value && value > maxScore)
                            classes.push(Msg.Error.toLowerCase());
                    }
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
                if(maxScore || maxScore == 0){
                    if(byNumeric_){
                       if(this.getNumericGradeValue() > maxScore)
                            res.push(Msg.Scores_exceeds);
                    }
                    else{
                        var value = parseFloat(this.getGradeValue());
                        if(value && value > maxScore)
                            res.push(Msg.Scores_exceeds);
                    }
                }
                if(!res.length)
                    return '';
                return res.join('<hr>');
            },

            String, function getNormalValue(){
                var value = this.getGradeValue();
                if(this.isDropped())
                    return Msg.Dropped;
                if(this.isExempt())
                    return Msg.Exempt;
                return value || '';
            },

            Boolean, function isEmptyGrade(){
                return !this.getGradeValue() &&
                        !this.isDropped() &&
                        !this.isExempt() &&
                        !this.isLate() &&
                        !this.isAbsent() &&
                        !this.isIncomplete()
            }
        ]);
});
