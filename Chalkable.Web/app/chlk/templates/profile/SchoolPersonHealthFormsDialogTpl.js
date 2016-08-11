REQUIRE('chlk.templates.common.SimpleObjectTpl');
REQUIRE('chlk.models.common.SimpleObject');

NAMESPACE('chlk.templates.profile', function () {
    "use strict";
    /** @class chlk.templates.profile.SchoolPersonHealthFormsDialogTpl*/

    CLASS(
         [ria.templates.TemplateBind('~/assets/jade/activities/profile/health-forms-dialog.jade')],
         [ria.templates.ModelBind(chlk.models.common.SimpleObject)],
        'SchoolPersonHealthFormsDialogTpl', EXTENDS(chlk.templates.common.SimpleObjectTpl), []);
});