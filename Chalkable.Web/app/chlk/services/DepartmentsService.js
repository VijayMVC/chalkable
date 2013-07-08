REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.departments.Department');

NAMESPACE('chlk.services', function () {
    "use strict";
    /** @class chlk.services.DepartmentsService*/
    CLASS(
        'DepartmentsService', EXTENDS(chlk.services.BaseService), [
            [[Number]],
            ria.async.Future, function getDepartments(pageIndex_) {
                return this.getPaginatedList('/app/data/departments.json', chlk.models.departments.Department, pageIndex_);
            }
        ])
});