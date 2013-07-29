REQUIRE('chlk.models.common.PaginatedList');
REQUIRE('chlk.models.id.DistrictId');

 NAMESPACE('chlk.models.school', function () {
     "use strict";

    /** @class chlk.models.school.SchoolListViewData*/
     CLASS(
        'SchoolListViewData', [
            chlk.models.common.PaginatedList, 'items',
            chlk.models.id.DistrictId, 'districtId',

            [[chlk.models.id.DistrictId, chlk.models.common.PaginatedList]],
            function $(districtId, items){
                this.setItems(items);
                this.setDistrictId(districtId);
            }

        ]);
});