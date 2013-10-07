REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.grading.GradingClassSummaryPart');

NAMESPACE('chlk.templates.grading', function () {
    "use strict";
    /** @class chlk.templates.grading.GradingClassSummaryPartTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/GradingClassSummaryPart.jade')],
        [ria.templates.ModelBind(chlk.models.grading.GradingClassSummaryPart)],
        'GradingClassSummaryPartTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.GradingClassSummaryItems), 'items'
        ]);
});
