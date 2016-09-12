REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.student.StudentInfo');

NAMESPACE('chlk.templates.profile', function () {
    "use strict";
    /** @class chlk.templates.profile.SchoolPersonInfoHealthFormsTpl*/

    CLASS(
         [ria.templates.TemplateBind('~/assets/jade/activities/profile/health-forms.jade')],
         [ria.templates.ModelBind(chlk.models.student.StudentInfo)],
        'SchoolPersonInfoHealthFormsTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.student.StudentHealthFormViewData), 'healthForms',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.SchoolPersonId, 'id'
        ]);
});