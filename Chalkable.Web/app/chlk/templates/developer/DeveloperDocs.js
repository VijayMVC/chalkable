REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.common.footers.DeveloperFooter');
REQUIRE('chlk.models.developer.DeveloperDocs');

NAMESPACE('chlk.templates.developer', function () {

    /** @class chlk.templates.developer.DeveloperDocs*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/developer/developer-docs.jade')],
        [ria.templates.ModelBind(chlk.models.developer.DeveloperDocs)],
        'DeveloperDocs', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            String, 'docsUrl'
        ])
});