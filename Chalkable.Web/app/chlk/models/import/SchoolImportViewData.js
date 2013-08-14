REQUIRE('chlk.models.id.DistrictId');
REQUIRE('chlk.models.import.School');
REQUIRE('chlk.models.common.PaginatedList');
NAMESPACE('chlk.models.import', function () {
    "use strict";
    /** @class chlk.models.import.SchoolImportViewData*/
    CLASS(
        'SchoolImportViewData', [
            chlk.models.id.DistrictId, 'districtId',
            ArrayOf(chlk.models.import.School), 'schools',
            [[chlk.models.id.DistrictId, ArrayOf(chlk.models.import.School)]],
            function $(districtId, schools){
                this.setDistrictId(districtId);
                this.setSchools(schools);
            }
        ]);
});
