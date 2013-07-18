REQUIRE('chlk.models.common.PaginatedList');
 
 NAMESPACE('chlk.models.school', function () {
     "use strict";

    /** @class chlk.models.school.SchoolListViewData*/
     CLASS(
        'SchoolListViewData', [
            chlk.models.common.PaginatedList, 'items',
            chlk.models.district.DistrictId, 'districtId'

        ]);
});