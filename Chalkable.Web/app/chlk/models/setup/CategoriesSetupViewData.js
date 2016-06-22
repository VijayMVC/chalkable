REQUIRE('chlk.models.announcement.ClassAnnouncementType');
REQUIRE('chlk.models.common.PageWithClasses');

NAMESPACE('chlk.models.setup', function () {
    "use strict";

    /** @class chlk.models.setup.CategoriesSetupViewData*/
    CLASS('CategoriesSetupViewData', EXTENDS(chlk.models.common.PageWithClasses), [

        ArrayOf(chlk.models.announcement.ClassAnnouncementType), 'categories',

        Boolean, 'ableEdit',

        [[chlk.models.classes.ClassesForTopBar, ArrayOf(chlk.models.announcement.ClassAnnouncementType), Boolean]],
        function $(classes_, categories_, ableEdit_){
            BASE(classes_);
            if(categories_)
                this.setCategories(categories_);
            if(ableEdit_)
                this.setAbleEdit(ableEdit_);
        }
    ]);
});
