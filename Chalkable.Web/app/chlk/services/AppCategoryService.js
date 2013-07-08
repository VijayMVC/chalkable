REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');

REQUIRE('chlk.models.apps.AppCategory');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.AppCategoryService */
    CLASS(
        'AppCategoryService', EXTENDS(chlk.services.BaseService), [
            [[Number]],
            ria.async.Future, function getCategories(pageIndex_) {
                return this.getPaginatedList('/app/data/appcategories.json', chlk.models.apps.AppCategory, pageIndex_);
            }
        ])
});