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

            Boolean, 'notAnnouncement',

            Boolean, 'readonly',

            Array, 'applicationsInGradeView',

            Number, 'gradingStyle',

            chlk.models.grading.Mapping, 'gradingMapping'
        ])
});