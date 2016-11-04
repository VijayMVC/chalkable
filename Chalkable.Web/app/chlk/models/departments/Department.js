REQUIRE('chlk.models.id.DepartmentId');

NAMESPACE('chlk.models.departments', function () {
    "use strict";
    /** @class chlk.models.departments.Department*/
    CLASS(
        'Department', [
            chlk.models.id.DepartmentId, 'id',
            String, 'name',
            String, 'keywords',
            Object, 'icon'
        ]);
});
