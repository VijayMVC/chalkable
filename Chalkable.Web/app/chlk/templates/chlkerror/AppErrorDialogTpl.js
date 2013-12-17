REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.apps.AppErrorViewData');

NAMESPACE('chlk.templates.chlkerror', function () {
    /** @class chlk.templates.chlkerror.AppErrorDialogTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/chlkerror/app-error-dialog.jade')],
        [ria.templates.ModelBind(chlk.models.apps.AppErrorViewData)],
        'AppErrorDialogTpl', EXTENDS(chlk.templates.ChlkTemplate),  [

            [ria.templates.ModelPropertyBind],
            String, 'developerEmail'
        ])
});