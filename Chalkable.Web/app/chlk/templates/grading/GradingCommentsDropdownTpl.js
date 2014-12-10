REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.grading.GradingComment');

NAMESPACE('chlk.templates.grading', function () {
    "use strict";
    /** @class chlk.templates.grading.GradingCommentsDropdownTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/GradingCommentsDropdown.jade')],
        [ria.templates.ModelBind(chlk.models.grading.GradingComment)],
        'GradingCommentsDropdownTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            String, 'comment'
        ]);
});
