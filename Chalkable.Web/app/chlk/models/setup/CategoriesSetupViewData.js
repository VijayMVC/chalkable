REQUIRE('chlk.models.announcement.ClassAnnouncementType');
REQUIRE('chlk.models.common.PageWithClasses');

NAMESPACE('chlk.models.setup', function () {
    "use strict";

    /** @class chlk.models.setup.CategoriesSetupViewData*/
    CLASS('CategoriesSetupViewData', EXTENDS(chlk.models.common.PageWithClasses), [

        ArrayOf(chlk.models.announcement.ClassAnnouncementType), 'categories',

        [[chlk.models.classes.ClassesForTopBar, ArrayOf(chlk.models.announcement.ClassAnnouncementType)]],
        function $(classes_, categories_){
            BASE(classes_);
            if(categories_)
                this.setCategories(categories_);
        }
    ]);
});
