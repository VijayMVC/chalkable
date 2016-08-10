REQUIRE('chlk.templates.common.PageWithClasses');
REQUIRE('chlk.models.common.PageWithClassesAndGradingPeriodsViewData');

NAMESPACE('chlk.templates.common', function () {

    /** @class chlk.templates.common.PageWithClassesAndGradingPeriodsTpl*/
    CLASS(
        [ria.templates.ModelBind(chlk.models.common.PageWithClassesAndGradingPeriodsViewData)],
        'PageWithClassesAndGradingPeriodsTpl', EXTENDS(chlk.templates.common.PageWithClasses), [
            [ria.templates.ModelPropertyBind],
            chlk.models.id.GradingPeriodId, 'gradingPeriodId'
        ])
});