REQUIRE('chlk.templates.profile.ClassProfileTpl');
REQUIRE('chlk.models.classes.BaseClassProfileViewData');

NAMESPACE('chlk.templates.classes.grading', function () {

    /** @class chlk.templates.classes.grading.ClassProfileGradingItemsBoxesTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/class/grading/ClassProfileGradingItemsBoxes.jade')],
        [ria.templates.ModelBind(chlk.models.classes.BaseClassProfileViewData)],
        'ClassProfileGradingItemsBoxesTpl', EXTENDS(chlk.templates.profile.ClassProfileTpl), [

        ])
});