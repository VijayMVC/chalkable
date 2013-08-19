REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.common.footers.DeveloperFooter');
REQUIRE('chlk.models.apps.Application');

NAMESPACE('chlk.templates.common.footers', function () {

    /** @class chlk.templates.common.footers.DeveloperFooter*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/common/footers/developer-footer.jade')],
        [ria.templates.ModelBind(chlk.models.common.footers.DeveloperFooter)],
        'DeveloperFooter', EXTENDS(chlk.templates.JadeTemplate), [

            [ria.templates.ModelPropertyBind],
            chlk.models.apps.Application, 'currentApp',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.apps.Application), 'developerApps'
        ])
});