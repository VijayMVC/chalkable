REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.attachment.FileCabinetViewData');

NAMESPACE('chlk.templates.announcement', function () {

    /** @class chlk.templates.announcement.FileCabinetTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/FileCabinetView.jade')],
        [ria.templates.ModelBind(chlk.models.attachment.FileCabinetViewData)],
        'FileCabinetTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AnnouncementId, 'announcementId',

            [ria.templates.ModelPropertyBind],
            chlk.models.announcement.AnnouncementTypeEnum, 'announcementType',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.PaginatedList, 'attachments',

            [ria.templates.ModelPropertyBind],
            chlk.models.attachment.SortAttachmentType, 'sortType',

            [ria.templates.ModelPropertyBind],
            String, 'filter',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AnnouncementAssignedAttributeId, 'assignedAttributeId',

        ]);
});