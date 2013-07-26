NAMESPACE('chlk.models.apps', function () {
    "use strict";

    /** @class chlk.models.apps.AppCategoryId*/
    IDENTIFIER('AppCategoryId');

    /** @class chlk.models.apps.AppCategory*/
    CLASS(
        'AppCategory', [
            chlk.models.apps.AppCategoryId, 'id',
            String, 'name',
            String, 'description'
        ]);
});
