REQUIRE('chlk.models.announcement.FeedAnnouncementViewData');
REQUIRE('chlk.templates.ChlkTemplate');

NAMESPACE('chlk.templates.announcement', function () {

    /** @class chlk.templates.announcement.FeedItemTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/FeedItem.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.FeedAnnouncementViewData)],
        'FeedItemTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            Number, 'ownerAttachmentsCount',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AnnouncementId, 'id',

            [ria.templates.ModelPropertyBind],
            String, 'className',

            [ria.templates.ModelPropertyBind],
            Number, 'applicationsCount',

            [ria.templates.ModelPropertyBind],
            Boolean, 'showGradingIcon',

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
            String, 'announcementTypeName',

            [ria.templates.ModelPropertyBind],
            String, 'title',

            [ria.templates.ModelPropertyBind],
            String, 'shortContent',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'expiresDate',

            [ria.templates.ModelPropertyBind],
            String, 'applicationName',

            [ria.templates.ModelPropertyBind],
            Number, 'studentsCount',

            [ria.templates.ModelPropertyBind],
            Number, 'studentsWithAttachmentsCount'
        ]);
});