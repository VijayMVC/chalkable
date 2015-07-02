REQUIRE('chlk.templates.announcement.ApplicationsAndAttachments');
REQUIRE('chlk.models.announcement.FeedAnnouncementViewData');
REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.id.StudentAnnouncementId');
REQUIRE('chlk.models.id.MarkingPeriodId');


NAMESPACE('chlk.templates.announcement', function () {

    /** @class chlk.templates.announcement.AnnouncementAppAttachments*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AnnouncementAppAttachments.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.FeedAnnouncementViewData)],
        'AnnouncementAppAttachments', EXTENDS(chlk.templates.announcement.ApplicationsAndAttachments), [

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AnnouncementId, 'id',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.attachment.Attachment), 'announcementAttachments',

            [ria.templates.ModelPropertyBind],
            chlk.models.announcement.AdminAnnouncementViewData, 'adminAnnouncementData',

            [ria.templates.ModelPropertyBind],
            chlk.models.announcement.LessonPlanViewData, 'lessonPlanData',

            [ria.templates.ModelPropertyBind],
            chlk.models.announcement.ClassAnnouncementViewData, 'classAnnouncementData',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.apps.AppAttachment), 'applications',

            [ria.templates.ModelPropertyBind],
            String, 'attachments',

            [ria.templates.ModelPropertyBind],
            String, 'applicationsIds',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.standard.Standard), 'standards',

            [ria.templates.ModelPropertyBind],
            chlk.models.announcement.StateEnum, 'state',

            [ria.templates.ModelPropertyBind],
            Boolean, 'needButtons',

            [ria.templates.ModelPropertyBind],
            Boolean, 'needDeleteButton',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AppId, 'assessmentApplicationId',

            String, function getStandardsUrlComponents() {
                return (this.standards || []).map(function (c, index) { return c.getUrlComponents(index); }).join('&')
            }
        ]);
});