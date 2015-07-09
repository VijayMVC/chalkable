REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.announcement.CategoryViewData');

NAMESPACE('chlk.templates.announcement', function () {

    /** @class chlk.templates.announcement.LessonPlanCategoryTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/LessonPlanCategory.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.CategoryViewData)],
        'LessonPlanCategoryTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            Number, 'id',

            [ria.templates.ModelPropertyBind],
            Number, 'lessonPlanCount',

            [ria.templates.ModelPropertyBind],
            String, 'name'
        ])
});
