REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.id.SchoolId');
REQUIRE('chlk.models.schoolYear.Year');

NAMESPACE('chlk.services', function () {
    "use strict";


    /** @class chlk.services.SchoolYearService */
    CLASS(
        'SchoolYearService', EXTENDS(chlk.services.BaseService), [
            [[chlk.models.id.SchoolId]],
            ria.async.Future, function list(schoolId_) {
                return this.get('SchoolYear/List.json', ArrayOf(chlk.models.schoolYear.Year), {
                    schoolId: schoolId_ && schoolId_.valueOf()
                });
            }
        ])
});