REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.common.BaseAttachViewData');

//TODO: DELETE

NAMESPACE('chlk.templates.common', function () {

    /** @class chlk.templates.common.BaseAttachTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/attach-apps-dialog.jade')],
        [ria.templates.ModelBind(chlk.models.common.BaseAttachViewData)],
        'BaseAttachTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            chlk.models.common.AttachOptionsViewData, 'attachOptions'
        ])
});