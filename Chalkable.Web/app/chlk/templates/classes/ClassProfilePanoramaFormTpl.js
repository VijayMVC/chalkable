REQUIRE('chlk.templates.classes.ClassProfilePanoramaTpl');

NAMESPACE('chlk.templates.classes', function () {

    /** @class chlk.templates.classes.ClassProfilePanoramaFormTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/class/ClassProfilePanoramaForm.jade')],
        [ria.templates.ModelBind(chlk.models.classes.BaseClassProfileViewData)],
        'ClassProfilePanoramaFormTpl', EXTENDS(chlk.templates.classes.ClassProfilePanoramaTpl), [

        ])
});