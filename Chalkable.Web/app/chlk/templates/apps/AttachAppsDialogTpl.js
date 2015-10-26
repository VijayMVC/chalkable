REQUIRE('chlk.templates.common.BaseAttachTpl');
REQUIRE('chlk.models.apps.InstalledAppsViewData');


NAMESPACE('chlk.templates.apps', function () {

    /** @class chlk.templates.apps.AttachAppsDialogTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/attach-apps-dialog.jade')],
        [ria.templates.ModelBind(chlk.models.apps.InstalledAppsViewData)],
        'AttachAppsDialogTpl', EXTENDS(chlk.templates.common.BaseAttachTpl), [

            [ria.templates.ModelPropertyBind],
            chlk.models.common.PaginatedList, 'apps',

            [ria.templates.ModelPropertyBind],
            String, 'appUrlAppend',

            [[chlk.models.apps.ApplicationForAttach]],
            String, function getAppIconToolTip(app){
                var res = null;

                if(app){
                    var notInstalledCount = app.getNotInstalledStudentsCount();
                    if (notInstalledCount > 0){
                        res = "This application isn't installed for " + notInstalledCount;
                        res += notInstalledCount > 1 ? ' students' : ' student';
                    }
                }
                return res;
            }
        ])
});