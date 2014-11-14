REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.id.StudentAnnouncementId');
REQUIRE('chlk.models.school.SchoolOption');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.announcement.ShortStudentAnnouncementViewData*/
    CLASS(
        UNSAFE, 'ShortStudentAnnouncementViewData', IMPLEMENTS(ria.serialize.IDeserializable), [

            VOID, function deserialize(raw) {
                this.announcementId = SJX.fromValue(raw.announcementid, chlk.models.id.AnnouncementId);
                this.studentId = SJX.fromValue(raw.studentid, chlk.models.id.SchoolPersonId);
                this.comment = SJX.fromValue(raw.comment, String);
                this.dropped = SJX.fromValue(raw.dropped, Boolean);
                this.gradeValue = SJX.fromValue(raw.gradevalue, String);
                this.numericGradeValue = SJX.fromValue(raw.numericgradevalue, Number);
                this.id = SJX.fromValue(raw.id, chlk.models.id.StudentAnnouncementId);
                this.state = SJX.fromValue(raw.state, Number);
                this.late = SJX.fromValue(raw.islate, Boolean);
                this.absent = SJX.fromValue(raw.isabsent, Boolean);
                this.unExcusedAbsent = SJX.fromValue(raw.isunexcusedabsent, Boolean);
                this.incomplete = SJX.fromValue(raw.isincomplete, Boolean);
                this.exempt = SJX.fromValue(raw.isexempt, Boolean);
                this.passed = SJX.fromValue(raw.ispassed, Boolean);
                this.complete = SJX.fromValue(raw.iscomplete, Boolean);
                this.includeInAverage = SJX.fromValue(raw.includeinaverage, Boolean);
            },

            chlk.models.id.AnnouncementId, 'announcementId',
            chlk.models.id.SchoolPersonId, 'studentId',
            String, 'comment',
            Boolean, 'dropped',
            String, 'gradeValue',
            Number, 'numericGradeValue',
            chlk.models.id.StudentAnnouncementId, 'id',
            Number, 'state',
            Boolean, 'late',
            Boolean, 'absent',
            Boolean, 'unExcusedAbsent',
            Boolean, 'incomplete',
            Boolean, 'exempt',
            Boolean, 'passed',
            Boolean, 'complete',
            Boolean, 'includeInAverage',

            [[chlk.models.id.StudentAnnouncementId, chlk.models.id.AnnouncementId, chlk.models.id.SchoolPersonId,
                Boolean, Boolean, Boolean, Boolean, Boolean, String, String, Boolean]],
            function $(id_, announcementId_, studentId_, dropped_, late_, exempt_, absent_, incomplete_, comment_, gradeValue_, includeInAverage_){
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
                if(includeInAverage_)
                    this.setIncludeInAverage(includeInAverage_);
            },

            Boolean, function needStrikeThrough(){
                return this.isDropped() && !!this.getGradeValue()
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
                if(this.isDropped() && !this.getGradeValue())
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
