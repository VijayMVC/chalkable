REQUIRE('chlk.templates.profile.ClassProfileTpl');
REQUIRE('chlk.models.classes.ClassProfileGradingViewData');

NAMESPACE('chlk.templates.classes', function () {
    "use strict";
    /** @class chlk.templates.classes.ClassProfileGradingTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/class/ClassProfileGrading.jade')],
        [ria.templates.ModelBind(chlk.models.classes.ClassProfileGradingViewData)],
        'ClassProfileGradingTpl', EXTENDS(chlk.templates.profile.ClassProfileTpl), [
            [ria.templates.ModelPropertyBind],
            chlk.models.classes.ClassGradingViewData, 'gradingPart'
        ]);
});