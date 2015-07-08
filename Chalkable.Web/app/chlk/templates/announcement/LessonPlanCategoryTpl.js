REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.common.NameId');

NAMESPACE('chlk.templates.announcement', function () {

    /** @class chlk.templates.announcement.LessonPlanCategoryTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/LessonPlanCategory.jade')],
        [ria.templates.ModelBind(chlk.models.common.NameId)],
        'LessonPlanCategoryTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            Object, 'id',

            [ria.templates.ModelPropertyBind],
            String, 'name'
        ])
});
