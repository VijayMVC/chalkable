REQUIRE('chlk.models.setup.CategoriesImportViewData');
REQUIRE('chlk.templates.ChlkTemplate');

NAMESPACE('chlk.templates.setup', function () {

    /** @class chlk.templates.setup.CategoriesImportTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/setup/CategoriesImport.jade')],
        [ria.templates.ModelBind(chlk.models.setup.CategoriesImportViewData)],
        'CategoriesImportTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.announcement.ClassAnnouncementType), 'categories',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.schoolYear.YearAndClasses), 'classesByYears',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassId, 'classId'
        ])
});
