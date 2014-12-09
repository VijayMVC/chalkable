REQUIRE('chlk.models.apps.ShortAppInstallViewData');
REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.templates.apps', function () {

    /** @class chlk.templates.apps.ShortInstallAppTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/ShortAppInstallDialog.jade')],
        [ria.templates.ModelBind(chlk.models.apps.ShortAppInstallViewData)],
        'ShortInstallAppTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.apps.AppMarketApplication, 'app',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassId, 'classId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AnnouncementId, 'announcementId'
        ]);
});