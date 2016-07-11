REQUIRE('chlk.models.setup.CategoriesSetupViewData');
REQUIRE('chlk.templates.common.PageWithClasses');

NAMESPACE('chlk.templates.setup', function () {

    /** @class chlk.templates.setup.CategoriesSetupTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/setup/CategoriesSetup.jade')],
        [ria.templates.ModelBind(chlk.models.setup.CategoriesSetupViewData)],
        'CategoriesSetupTpl', EXTENDS(chlk.templates.common.PageWithClasses), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.announcement.ClassAnnouncementType), 'categories',

            [ria.templates.ModelPropertyBind],
            Boolean, 'ableEdit',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.schoolYear.YearAndClasses), 'classesByYears'
        ])
});
