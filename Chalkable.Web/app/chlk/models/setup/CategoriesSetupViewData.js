REQUIRE('chlk.models.announcement.ClassAnnouncementType');
REQUIRE('chlk.models.common.PageWithClasses');

NAMESPACE('chlk.models.setup', function () {
    "use strict";

    /** @class chlk.models.setup.CategoriesSetupViewData*/
    CLASS('CategoriesSetupViewData', EXTENDS(chlk.models.common.PageWithClasses), [

        ArrayOf(chlk.models.announcement.ClassAnnouncementType), 'categories',

        Boolean, 'ableEdit',

        ArrayOf(chlk.models.schoolYear.YearAndClasses), 'classesByYears',

        [[chlk.models.classes.ClassesForTopBar, ArrayOf(chlk.models.announcement.ClassAnnouncementType), Boolean, ArrayOf(chlk.models.schoolYear.YearAndClasses)]],
        function $(classes_, categories_, ableEdit_, classesByYears_){
            BASE(classes_);
            if(categories_)
                this.setCategories(categories_);
            if(ableEdit_)
                this.setAbleEdit(ableEdit_);
            if(classesByYears_)
                this.setClassesByYears(classesByYears_);
        }
    ]);
});
