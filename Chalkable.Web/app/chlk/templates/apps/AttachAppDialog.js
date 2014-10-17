REQUIRE('chlk.templates.PaginatedList');
REQUIRE('chlk.models.common.PaginatedList');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.apps.InstalledAppsViewData');


NAMESPACE('chlk.templates.apps', function () {

    /** @class chlk.templates.apps.AttachAppDialog*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/attach-app-dialog.jade')],
        [ria.templates.ModelBind(chlk.models.apps.InstalledAppsViewData)],
        'AttachAppDialog', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.id.SchoolPersonId, 'teacherId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AnnouncementId, 'announcementId',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.PaginatedList, 'apps',

            [[chlk.models.apps.ApplicationForAttach]],
            String, function getAppIconToolTip(app){
                var res = null
                if(app && app.getNotInstalledStudentsCount() > 0){
                    res = app.getNotInstalledStudentsCount();
                    res += res > 1 ? ' student have ' : ' students has ';
                    res += ' no installed current application yet';
                }
                return res;
            }
        ])
});