REQUIRE('chlk.models.announcement.ClassAnnouncementType');

NAMESPACE('chlk.models.setup', function () {
    "use strict";

    /** @class chlk.models.setup.CategoriesImportViewData*/
    CLASS('CategoriesImportViewData', [

        ArrayOf(chlk.models.announcement.ClassAnnouncementType), 'categories',
        
        ArrayOf(chlk.models.schoolYear.YearAndClasses), 'classesByYears',
        
        chlk.models.id.ClassId, 'classId',

        [[chlk.models.id.ClassId, ArrayOf(chlk.models.schoolYear.YearAndClasses), ArrayOf(chlk.models.announcement.ClassAnnouncementType)]],
        function $(classId_, classesByYears_, categories_){
            BASE();
            classId_ && this.setClassId(classId_);
            classesByYears_ && this.setClassesByYears(classesByYears_);
            categories_ && this.setCategories(categories_);
        }
    ]);
});
