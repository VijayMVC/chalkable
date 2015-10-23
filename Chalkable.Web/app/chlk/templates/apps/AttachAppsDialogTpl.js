REQUIRE('chlk.templates.PaginatedList');
REQUIRE('chlk.models.common.PaginatedList');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.apps.InstalledAppsViewData');


NAMESPACE('chlk.templates.apps', function () {

    /** @class chlk.templates.apps.AttachAppsDialogTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/attach-apps-dialog.jade')],
        [ria.templates.ModelBind(chlk.models.apps.InstalledAppsViewData)],
        'AttachAppsDialogTpl', EXTENDS(chlk.templates.ChlkTemplate), [

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
            Boolean, 'showApps',

            [ria.templates.ModelPropertyBind],
            String, 'announcementTypeName',

            [ria.templates.ModelPropertyBind],
            chlk.models.announcement.AnnouncementTypeEnum, 'announcementType',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AnnouncementAssignedAttributeId, 'assignedAttributeId',

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