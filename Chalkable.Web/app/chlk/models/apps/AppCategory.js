REQUIRE('chlk.models.id.AppCategoryId');

NAMESPACE('chlk.models.apps', function () {
    "use strict";
    /** @class chlk.models.apps.AppCategory*/
    CLASS(
        'AppCategory', [
            chlk.models.id.AppCategoryId, 'id',
            String, 'name',
            String, 'description'
        ]);
});
