REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.announcement.StudentAnnouncement');

NAMESPACE('chlk.templates.announcement', function () {

    /** @class chlk.templates.announcement.StudentAnnouncement*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/StudentAnnouncement.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.StudentAnnouncement)],
        'StudentAnnouncement', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.id.AnnouncementId, 'announcementId',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.attachment.Attachment), 'attachments',

            [ria.templates.ModelPropertyBind],
            String, 'comment',

            [ria.templates.ModelPropertyBind],
            String, 'gradeValue',

            [ria.templates.ModelPropertyBind],
            Number, 'numericGradeValue',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.StudentAnnouncementId, 'id',

            [ria.templates.ModelPropertyBind],
            Number, 'state',

            [ria.templates.ModelPropertyBind],
            chlk.models.people.User, 'studentInfo',

            [ria.templates.ModelPropertyBind],
            chlk.models.people.User, 'owner',

            Boolean, 'readonly',

            Array, 'applicationsInGradeView',

            Number, 'gradingStyle',

            chlk.models.grading.Mapping, 'gradingMapping',

            [ria.templates.ModelPropertyBind],
            Boolean, 'late',

            [ria.templates.ModelPropertyBind],
            Boolean, 'absent',

            [ria.templates.ModelPropertyBind],
            Boolean, 'incomplete',

            [ria.templates.ModelPropertyBind],
            Boolean, 'exempt',

            [ria.templates.ModelPropertyBind],
            Boolean, 'dropped',

            [ria.templates.ModelPropertyBind],
            Boolean, 'passed',

            [ria.templates.ModelPropertyBind],
            Boolean, 'complete',

            Number, 'maxScore',

            Object, function getNormalValue(){
                var value = this.getGradeValue();
                if(this.isDropped())
                    return Msg.Dropped;
                if(this.isExempt())
                    return Msg.Exempt;
                return value;
            },

            String, function getTooltipText(){
                var res = [];
                if(this.isLate())
                    res.push(Msg.Late);
                if(this.isIncomplete())
                    res.push(Msg.Incomplete);
                if(this.isAbsent())
                    res.push(Msg.Student_marked_absent);
                if(!res.length)
                    return '';
                return res.join('<hr>');
            },

            String, function getAlertClass(){
                if(this.isLate()){
                    if(!this.isIncomplete())
                        return Msg.Late.toLowerCase();
                    if(this.isIncomplete())
                        return Msg.Multiple.toLowerCase();
                }
                else
                    if(this.isIncomplete())
                        return Msg.Incomplete.toLowerCase();
                    else
                        if(this.isAbsent())
                            return Msg.Absent.toLowerCase();
                return '';
            },

            Boolean, function isEmptyGrade(){
                return !(this.getGradeValue() || this.isLate() || this.isAbsent() || this.isDropped() || this.isExempt() || this.isIncomplete());
            },

            String, function getGradeInputClass(){
                var normalValue = this.getNormalValue();
                return this.isGradeDisabled() ? "disabled-grade" : "with-grid-focus" + ((!normalValue && normalValue != 0) ? " empty-grade" : "") +
                    (this.isEmptyGrade() ? " able-fill-all" : "");
            },

            Object, function isGradeDisabled(){
                return this.isDropped() || this.isExempt();
            }
        ])
});