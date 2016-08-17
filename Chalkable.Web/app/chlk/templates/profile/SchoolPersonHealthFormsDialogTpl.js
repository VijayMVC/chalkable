REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.student.VerifyHealthFormViewData');

NAMESPACE('chlk.templates.profile', function () {
    "use strict";
    /** @class chlk.templates.profile.SchoolPersonHealthFormsDialogTpl*/

    CLASS(
         [ria.templates.TemplateBind('~/assets/jade/activities/profile/health-forms-dialog.jade')],
         [ria.templates.ModelBind(chlk.models.student.VerifyHealthFormViewData)],
        'SchoolPersonHealthFormsDialogTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            String, 'requestId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.HealthFormId, 'healthFormId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.SchoolPersonId, 'studentId',

            [ria.templates.ModelPropertyBind],
            String, 'documentUrl',
        ]);
});