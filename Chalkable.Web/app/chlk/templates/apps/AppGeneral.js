REQUIRE('chlk.models.apps.Application');
REQUIRE('chlk.models.apps.AppInfoViewData');
REQUIRE('chlk.models.common.NameId');
REQUIRE('chlk.models.id.AppGradeLevelId');
REQUIRE('chlk.models.apps.AppGradeLevel');

NAMESPACE('chlk.templates.apps', function () {

    /** @class chlk.templates.apps.AppInfo*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/AppInfo.jade')],
        [ria.templates.ModelBind(chlk.models.apps.AppInfoViewData)],
        'AppInfo', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.apps.Application, 'app',
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.apps.Application), 'apps',
            [ria.templates.ModelPropertyBind],
            Boolean, 'empty',
            [ria.templates.ModelPropertyBind],
            Boolean, 'draft',
            [ria.templates.ModelPropertyBind],
            Boolean, 'readOnly',
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.apps.AppCategory), 'categories',
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.apps.AppGradeLevel), 'gradeLevels',
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.apps.AppPermission), 'permissions'

        ])
});