REQUIRE('chlk.templates.common.BaseAttachTpl');
REQUIRE('chlk.models.attachment.FileCabinetViewData');

NAMESPACE('chlk.templates.announcement', function () {

    /** @class chlk.templates.announcement.FileCabinetTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/FileCabinetView.jade')],
        [ria.templates.ModelBind(chlk.models.attachment.FileCabinetViewData)],
        'FileCabinetTpl', EXTENDS(chlk.templates.common.BaseAttachTpl), [

            [ria.templates.ModelPropertyBind],
            chlk.models.common.PaginatedList, 'attachments',

            [ria.templates.ModelPropertyBind],
            chlk.models.attachment.SortAttachmentType, 'sortType',

            [ria.templates.ModelPropertyBind],
            String, 'filter'
        ]);
});