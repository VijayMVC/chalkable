NAMESPACE('chlk.models.apps', function () {
    "use strict";
    /** @class chlk.models.apps.AppCategory*/
    CLASS(
        'AppCategory', [
            Number, 'id',
            String, 'name',
            String, 'description'
        ]);
});
