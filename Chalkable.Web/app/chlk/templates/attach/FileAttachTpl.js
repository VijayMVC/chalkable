REQUIRE('chlk.templates.common.BaseAttachTpl');
REQUIRE('chlk.models.common.BaseAttachViewData');

NAMESPACE('chlk.templates.attach', function () {

    /** @class chlk.templates.attach.FileAttachTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/attach/file-attach-dialog.jade')],
        [ria.templates.ModelBind(chlk.models.common.BaseAttachViewData)],
        'FileAttachTpl', EXTENDS(chlk.templates.common.BaseAttachTpl), [
            [ria.templates.ModelPropertyBind],
            String, 'requestId'
        ])
});
