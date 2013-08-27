REQUIRE('chlk.models.id.DistrictId');
NAMESPACE('chlk.models.import', function () {
    "use strict";
    /** @class chlk.models.import.ImportTaskData*/
    CLASS(
        'ImportTaskData', [
            Number, 'sisSchoolId',
            Number, 'sisSchoolYearId',
            chlk.models.id.DistrictId, 'districtId'
        ]);
});
