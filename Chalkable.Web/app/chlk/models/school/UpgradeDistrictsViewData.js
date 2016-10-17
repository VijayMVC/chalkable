REQUIRE('chlk.models.common.PaginatedList');

NAMESPACE('chlk.models.school', function(){

    /** @class chlk.models.school.UpgradeDistrictsViewData*/

    CLASS('UpgradeDistrictsViewData', [

        chlk.models.common.PaginatedList, 'districts',
        String, 'filter',

        [[chlk.models.common.PaginatedList, String]],
        function $(districts_, filter_){
            BASE();
            if(districts_)
                this.setDistricts(districts_);
            if(filter_)
                this.setFilter(filter_);
        }
    ]);
});