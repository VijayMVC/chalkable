REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.grading.GradingComments');

NAMESPACE('chlk.templates.grading', function () {
    "use strict";
    /** @class chlk.templates.grading.GradingCommentsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/GradingComments.jade')],
        [ria.templates.ModelBind(chlk.models.grading.GradingComments)],
        'GradingCommentsTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(String), 'comments'
        ]);
});
