REQUIRE('chlk.templates.ChlkTemplate');

REQUIRE('chlk.models.apps.MiniQuizViewData');

NAMESPACE('chlk.templates.apps', function () {
    /** @class chlk.templates.apps.MiniQuizDialog*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/mini-quiz-dialog.jade')],
        [ria.templates.ModelBind(chlk.models.apps.MiniQuizViewData)],
        'MiniQuizDialog', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(String), 'ccStandardCodes',

            [ria.templates.ModelPropertyBind],
            String, 'currentStandardCode',

            [ria.templates.ModelPropertyBind],
            chlk.models.apps.Application, 'applicationInfo',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.apps.Application), 'installedApplications',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.apps.Application), 'recommendedApplications'
        ])
});