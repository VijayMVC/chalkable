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
            Boolean, 'dropped',

            [ria.templates.ModelPropertyBind],
            Number, 'gradeValue',

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
            Boolean, 'incomplete',

            [ria.templates.ModelPropertyBind],
            Boolean, 'exempt',

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
                return '';
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
        ])
});