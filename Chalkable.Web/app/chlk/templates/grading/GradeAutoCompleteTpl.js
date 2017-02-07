REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.common.Array');

NAMESPACE('chlk.templates.grading', function () {
    "use strict";
    /** @class chlk.templates.grading.GradeAutoCompleteTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/GradeAutoComplete.jade')],
        [ria.templates.ModelBind(chlk.models.common.Array)],
        'GradeAutoCompleteTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            Array, 'items'
        ]);
});
