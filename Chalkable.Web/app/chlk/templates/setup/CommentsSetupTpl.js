REQUIRE('chlk.models.setup.CommentsSetupViewData');
REQUIRE('chlk.templates.ChlkTemplate');

NAMESPACE('chlk.templates.setup', function () {

    /** @class chlk.templates.setup.CommentsSetupTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/setup/CommentsSetup.jade')],
        [ria.templates.ModelBind(chlk.models.setup.CommentsSetupViewData)],
        'CommentsSetupTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.TeacherCommentViewData), 'comments',

            [ria.templates.ModelPropertyBind],
            Boolean, 'ableEdit'
        ])
});
