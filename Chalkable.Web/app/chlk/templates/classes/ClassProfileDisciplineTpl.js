REQUIRE('chlk.templates.profile.ClassProfileTpl');
REQUIRE('chlk.models.classes.BaseClassProfileViewData');

NAMESPACE('chlk.templates.classes', function () {

    /** @class chlk.templates.classes.ClassProfileDisciplineTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/class/ClassProfileDiscipline.jade')],
        [ria.templates.ModelBind(chlk.models.classes.BaseClassProfileViewData)],
        'ClassProfileDisciplineTpl', EXTENDS(chlk.templates.profile.ClassProfileTpl), [

        ])
});