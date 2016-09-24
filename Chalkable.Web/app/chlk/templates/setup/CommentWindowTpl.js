REQUIRE('chlk.models.grading.TeacherCommentViewData');
REQUIRE('chlk.templates.ChlkTemplate');

NAMESPACE('chlk.templates.setup', function () {

    /** @class chlk.templates.setup.CommentWindowTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/setup/CommentWindow.jade')],
        [ria.templates.ModelBind(chlk.models.grading.TeacherCommentViewData)],
        'CommentWindowTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            chlk.models.id.TeacherCommentId, 'commentId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.SchoolPersonId, 'teacherId',

            [ria.templates.ModelPropertyBind],
            Boolean, 'system',

            [ria.templates.ModelPropertyBind],
            String, 'comment',

            [ria.templates.ModelPropertyBind],
            Boolean, 'ableEdit'
        ])
});
