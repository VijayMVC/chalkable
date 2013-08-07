REQUIRE('chlk.models.apps.Application');
REQUIRE('chlk.models.apps.AppInfoViewData');
REQUIRE('chlk.models.common.NameId');

NAMESPACE('chlk.templates.apps', function () {

    /** @class chlk.templates.apps.AppInfo*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/AppInfo.jade')],
        [ria.templates.ModelBind(chlk.models.apps.AppInfoViewData)],
        'AppInfo', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.apps.Application, 'app',
            [ria.templates.ModelPropertyBind],
            Boolean, 'empty',
            [ria.templates.ModelPropertyBind],
            Boolean, 'readOnly',
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.apps.AppCategory), 'categories',
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.common.NameId), 'gradeLevels'

        ])
});