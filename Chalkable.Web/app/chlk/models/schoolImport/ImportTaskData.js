REQUIRE('chlk.models.id.DistrictId');
NAMESPACE('chlk.models.schoolImport', function () {
    "use strict";
    /** @class chlk.models.schoolImport.ImportTaskData*/
    CLASS(
        'ImportTaskData', [
            Number, 'sisSchoolId',
            Number, 'sisSchoolYearId',
            chlk.models.id.DistrictId, 'districtId'
        ]);
});
