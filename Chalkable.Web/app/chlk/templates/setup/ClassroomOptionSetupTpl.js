REQUIRE('chlk.models.setup.ClassroomOptionSetupViewData');
REQUIRE('chlk.templates.common.PageWithClasses');

NAMESPACE('chlk.templates.setup', function () {

    /** @class chlk.templates.setup.ClassroomOptionSetupTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/setup/ClassroomOptionSetup.jade')],
        [ria.templates.ModelBind(chlk.models.setup.ClassroomOptionSetupViewData)],
        'ClassroomOptionSetupTpl', EXTENDS(chlk.templates.common.PageWithClasses), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.GradingScale), 'scales',

            [ria.templates.ModelPropertyBind],
            Boolean, 'ableCopy',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.schoolYear.YearAndClasses), 'classesByYears',

            [ria.templates.ModelPropertyBind],
            chlk.models.grading.ClassroomOptionViewData, 'classroomOptions'
        ])
});
