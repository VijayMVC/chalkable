REQUIRE('chlk.models.departments.Department');
NAMESPACE('chlk.models.departments', function () {
    "use strict";
    /** @class chlk.models.departments.DepartmentsList*/
    CLASS(
        'DepartmentsList', [
            ArrayOf(chlk.models.departments.Department), 'items'
        ]);
});