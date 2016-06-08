REQUIRE('chlk.models.common.PaginatedList');
REQUIRE('chlk.models.id.DistrictId');

 NAMESPACE('chlk.models.school', function () {
     "use strict";

    /** @class chlk.models.school.SchoolListViewData*/
     CLASS(
        'SchoolListViewData', [
            chlk.models.common.PaginatedList, 'paginatedSchools',
            chlk.models.id.DistrictId, 'districtId',

            [[chlk.models.id.DistrictId, chlk.models.common.PaginatedList]],
            function $(districtId, paginatedSchools){
                BASE();

                VALIDATE_ARG('paginatedSchools', [ClassOf(chlk.models.school.School)], paginatedSchools.getItemClass());
                VALIDATE_ARG('paginatedSchools', [ArrayOf(chlk.models.school.School)], paginatedSchools.getItems());

                this.setPaginatedSchools(paginatedSchools);
                this.setDistrictId(districtId);
            }

        ]);
});