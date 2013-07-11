NAMESPACE('chlk.models.departments', function () {
    "use strict";
    /** @class chlk.models.departments.Department*/
    CLASS(
        'Department', [
            Number, 'id',
            String, 'name',
            ArrayOf(String), 'keywords'
        ]);
});
