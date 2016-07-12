REQUIRE('chlk.templates.ChlkTemplate');

REQUIRE('chlk.models.attachment.Attachment');

NAMESPACE('chlk.templates', function () {

    /** @class chlk.templates.LoadingImageTpl*/
    CLASS(
        [ria.templates.ModelBind(chlk.models.attachment.Attachment)],
        [ria.templates.TemplateBind('~/assets/jade/common/LoadingImage.jade')],
        'LoadingImageTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            String, 'thumbnailUrl',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AttachmentId, 'id',

            [ria.templates.ModelPropertyBind],
            String, 'url',

            [ria.templates.ModelPropertyBind],
            String, 'pictureId',

            [ria.templates.ModelPropertyBind],
            String, 'name',

            [ria.templates.ModelPropertyBind],
            chlk.models.attachment.AttachmentTypeEnum, 'type'
        ])
});