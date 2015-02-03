REQUIRE('chlk.templates.ChlkTemplate');

REQUIRE('chlk.models.apps.MiniQuizViewData');

NAMESPACE('chlk.templates.apps', function () {

    /** @class chlk.templates.apps.MiniQuizDialog*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/mini-quiz-dialog.jade')],
        [ria.templates.ModelBind(chlk.models.apps.MiniQuizViewData)],
        'MiniQuizDialog', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.standard.Standard), 'standards',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.StandardId, 'currentStandardId',

            [ria.templates.ModelPropertyBind],
            chlk.models.apps.Application, 'applicationInfo',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.apps.Application), 'installedApplications',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.apps.Application), 'recommendedApplications',

            [ria.templates.ModelPropertyBind],
            String, 'authorizationCode',

            chlk.models.standard.Standard, function getCurrentStandard() {
                var c = this.getCurrentStandardId();
                return this.standards.filter(function (_) { return _.getStandardId() == c; })[0];
            }
        ])
});