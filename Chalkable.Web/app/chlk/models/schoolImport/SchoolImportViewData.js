REQUIRE('chlk.models.id.DistrictId');
REQUIRE('chlk.models.schoolImport.School');
REQUIRE('chlk.models.common.PaginatedList');
NAMESPACE('chlk.models.schoolImport', function () {
    "use strict";
    /** @class chlk.models.schoolImport.SchoolImportViewData*/
    CLASS(
        'SchoolImportViewData', [
            chlk.models.id.DistrictId, 'districtId',
            ArrayOf(chlk.models.schoolImport.School), 'schools',
            [[chlk.models.id.DistrictId, ArrayOf(chlk.models.schoolImport.School)]],
            function $(districtId, schools){
                BASE();
                this.setDistrictId(districtId);
                this.setSchools(schools);
            }
        ]);
});
