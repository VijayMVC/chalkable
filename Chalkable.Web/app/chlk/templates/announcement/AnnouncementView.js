REQUIRE('chlk.templates.announcement.AnnouncementAppAttachments');
REQUIRE('chlk.models.announcement.AnnouncementView');
REQUIRE('chlk.templates.announcement.AnnouncementQnAs');


NAMESPACE('chlk.templates.announcement', function () {

    /** @class chlk.templates.announcement.AnnouncementView*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AnnouncementView.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.AnnouncementView)],
        'AnnouncementView', EXTENDS(chlk.templates.announcement.AnnouncementAppAttachments), [

            [ria.templates.ModelPropertyBind],
            chlk.models.announcement.StudentAnnouncements, 'studentAnnouncements',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.announcement.AnnouncementComment), 'announcementComments',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.apps.AppAttachment), 'gradeViewApps',

            [ria.templates.ModelPropertyBind],
            Number, 'grade',

            [ria.templates.ModelPropertyBind],
            Array, 'autoGradeApps',

            [ria.templates.ModelPropertyBind],
            Boolean, 'ableEdit',

            [ria.templates.ModelPropertyBind],
            String, 'announcementTypeName',

            [ria.templates.ModelPropertyBind],
            String, 'title',

            [ria.templates.ModelPropertyBind],
            String, 'comment',

            [ria.templates.ModelPropertyBind],
            chlk.models.people.User, 'owner',

            [ria.templates.ModelPropertyBind],
            String, 'content',

            [ria.templates.ModelPropertyBind],
            Boolean, 'annOwner',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.announcement.AdminAnnouncementRecipient), 'recipients',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.announcement.AnnouncementQnA), 'announcementQnAs',

            [ria.templates.ModelPropertyBind],
            Number, 'applicationsCount',

            [ria.templates.ModelPropertyBind],
            Boolean, 'exempt',

            [ria.templates.ModelPropertyBind],
            Boolean, 'hasAccessToLE',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.PaginatedList, 'students',

            /*[ria.templates.ModelPropertyBind],
            Number, 'ownerAttachmentsCount',

            [ria.templates.ModelPropertyBind],
            Boolean, 'showGradingIcon',

            [ria.templates.ModelPropertyBind],
            Number, 'applicationsCount',

            [ria.templates.ModelPropertyBind],
            String, 'className',

            [ria.templates.ModelPropertyBind],
            Number, 'announcementTypeId',

            [ria.templates.ModelPropertyBind],
            Boolean, 'complete',

            [ria.templates.ModelPropertyBind],
            Number, 'gradingStudentsCount',

            [ria.templates.ModelPropertyBind],
            Number, 'chalkableAnnouncementType',

            [ria.templates.ModelPropertyBind],
            Boolean, 'adminAnnouncement',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassId, 'classId',

            [ria.templates.ModelPropertyBind],
            String, 'shortContent',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'expiresDate',

            [ria.templates.ModelPropertyBind],
            String, 'applicationName',



             [ria.templates.ModelPropertyBind],
             ArrayOf(chlk.models.grading.AlphaGrade), 'alphaGrades',

             [ria.templates.ModelPropertyBind],
             ArrayOf(chlk.models.grading.AlternateScore), 'alternateScores',*/


            Number, function getAutoGradeCount(){
                return (this.getStudentAnnouncements().getItems() || []).filter(function(item){
                    return item.getState() == 0;
                }).length;
            },

            Boolean, function isStudentGraded(){
                var grade = this.getGrade();
                return this.getClassAnnouncementData() && (this.getClassAnnouncementData().isDropped() || this.isExempt() || grade >= 0);
            },

            String, function displayStudentGradeValue(){
                if(!this.getClassAnnouncementData()) return '';
                if(this.getClassAnnouncementData().isDropped()) return Msg.Dropped;
                if(this.isExempt()) return Msg.Exempt;
                var grade = this.getGrade();
                if(grade || grade == 0) return grade.toString();
                return '';
            }
        ])
});