REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.standard.Standard');

NAMESPACE('chlk.templates.standard', function () {

    /** @class chlk.templates.standard.StandardAutoCompleteTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/StandardAutoComplete.jade')],
        [ria.templates.ModelBind(chlk.models.standard.Standard)],
        'StandardAutoCompleteTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            String, 'name',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.StandardId, 'standardId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.StandardSubjectId, 'subjectId'
        ])
});

