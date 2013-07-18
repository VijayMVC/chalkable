NAMESPACE('chlk.models.departments', function () {
    "use strict";

    /** @class chlk.models.departments.DepartmentId*/
    IDENTIFIER('DepartmentId');

    /** @class chlk.models.departments.Department*/
    CLASS(
        'Department', [
            chlk.models.departments.DepartmentId, 'id',
            String, 'name',
            //ArrayOf(String), 'keywords'
            String, 'keywords'
        ]);
});
