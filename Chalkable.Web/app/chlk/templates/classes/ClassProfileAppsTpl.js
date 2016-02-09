REQUIRE('chlk.templates.profile.ClassProfileTpl');
REQUIRE('chlk.models.classes.ClassProfileAppsViewData');

NAMESPACE('chlk.templates.classes', function () {
    "use strict";
    /** @class chlk.templates.classes.ClassProfileAppsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/class/ClassProfileAppsView.jade')],
        [ria.templates.ModelBind(chlk.models.classes.ClassProfileAppsViewData)],
        'ClassProfileAppsTpl', EXTENDS(chlk.templates.profile.ClassProfileTpl), [
        ]);
});