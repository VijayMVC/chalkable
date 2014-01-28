REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.common.attachments.BaseAttachmentViewData');

NAMESPACE('chlk.templates.common.attachments', function () {
    /** @class chlk.templates.common.attachments.AttachmentDialogTpl*/

    ASSET('~/assets/jade/common/attachments/attachments-shared.jade')();

    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/common/attachments/AttachmentDialog.jade')],
        [ria.templates.ModelBind(chlk.models.common.attachments.BaseAttachmentViewData)],
        'AttachmentDialogTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.common.attachments.ToolbarButton), 'toolbarButtons',

            [ria.templates.ModelPropertyBind],
            String, 'url'
        ])
});