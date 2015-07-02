REQUIRE('chlk.templates.PaginatedList');
REQUIRE('chlk.models.common.PaginatedList');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.apps.InstalledAppsViewData');


NAMESPACE('chlk.templates.apps', function () {

    /** @class chlk.templates.apps.AttachDialogTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/attach-dialog.jade')],
        [ria.templates.ModelBind(chlk.models.apps.InstalledAppsViewData)],
        'AttachDialogTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AnnouncementId, 'announcementId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassId, 'classId',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.PaginatedList, 'apps',

            [ria.templates.ModelPropertyBind],
            String, 'appUrlAppend',


            [ria.templates.ModelPropertyBind],
            chlk.models.id.AppId, 'assessmentAppId',

            [ria.templates.ModelPropertyBind],
            Boolean, 'fileCabinetEnabled',

            [ria.templates.ModelPropertyBind],
            Boolean, 'standardAttachEnabled',

            [ria.templates.ModelPropertyBind],
            Boolean, 'attributesEnabled',

            [ria.templates.ModelPropertyBind],
            Boolean, 'showApps',

            [ria.templates.ModelPropertyBind],
            String, 'announcementTypeName',

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