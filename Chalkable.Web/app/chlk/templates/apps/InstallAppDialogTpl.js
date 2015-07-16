REQUIRE('chlk.models.apps.AppMarketInstallViewData');
REQUIRE('chlk.templates.ChlkTemplate');

NAMESPACE('chlk.templates.apps', function () {

    /** @class chlk.templates.apps.InstallAppDialogTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/app-install-dialog.jade')],
        [ria.templates.ModelBind(chlk.models.apps.AppMarketInstallViewData)],


        'InstallAppDialogTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.apps.AppMarketApplication, 'app',

            [ria.templates.ModelPropertyBind],
            Boolean, 'alreadyInstalled',

            [ria.templates.ModelPropertyBind],
            chlk.models.classes.AllSchoolsActiveClasses, 'allClasses',

            [[chlk.models.id.ClassId]],
            Boolean, function isAlreadyInstalled_(classId) {
                return this.app.getInstalledForGroups().some(function (_) {
                    return _.getGroupType() == chlk.models.apps.AppInstallGroupTypeEnum.CLAZZ
                        && _.getId().valueOf() == classId.valueOf()
                        && _.isInstalled();
                })
            },

            function getOrderedClasses_() {
                return this.allClasses.getClasses().sort(function (_1, _2) {
                    var a = _1.getFullClassName(), b = _2.getFullClassName();
                    return  a < b ? -1 : a > b ? 1 : 0;
                })
            }
        ])
});