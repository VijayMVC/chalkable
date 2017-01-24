REQUIRE('chlk.templates.common.BaseAttachTpl');
REQUIRE('chlk.models.common.BaseAttachViewData');

NAMESPACE('chlk.templates.announcement', function () {

    /** @class chlk.templates.announcement.FileAttachTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/attach-files-dialog.jade')],
        [ria.templates.ModelBind(chlk.models.common.BaseAttachViewData)],
        'FileAttachTpl', EXTENDS(chlk.templates.common.BaseAttachTpl), [

        ])
});