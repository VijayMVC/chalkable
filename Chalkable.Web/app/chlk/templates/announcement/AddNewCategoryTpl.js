REQUIRE('chlk.templates.common.SimpleObjectTpl');

NAMESPACE('chlk.templates.announcement', function () {

    /** @class chlk.templates.announcement.AddNewCategoryTpl*/
    CLASS(
        [ria.templates.ModelBind(chlk.models.common.SimpleObject)],
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AddNewCategory.jade')],
        'AddNewCategoryTpl', EXTENDS(chlk.templates.common.SimpleObjectTpl), [])
});