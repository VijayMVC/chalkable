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
            chlk.models.id.ClassId, 'classId',

            function getClassesForSelect(){
                var items = this.getClassesByYears(), res = [];
                items.forEach(function(item){
                    var classes = item.getClasses();
                    if(classes.length){
                        res.push({
                            name: item.getSchoolYear().getName(),
                            values: classes.map(function(clazz){
                                return {
                                    name: clazz.getFullClassName(),
                                    id: clazz.getId().valueOf()
                                }
                            })
                        })
                    }
                });
                return res;
            }
        ])
});
