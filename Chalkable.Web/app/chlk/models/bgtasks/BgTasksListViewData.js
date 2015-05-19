REQUIRE('chlk.models.bgtasks.BgTask');
REQUIRE('chlk.models.bgtasks.BgTaskState');
REQUIRE('chlk.models.bgtasks.BgTaskType');


NAMESPACE('chlk.models.bgtasks', function () {
    "use strict";
    /** @class chlk.models.bgtasks.BgTasksListViewData*/
    CLASS(
        'BgTasksListViewData', [
            chlk.models.common.PaginatedList, 'bgTasks',

            ArrayOf(chlk.models.bgtasks.BgTaskState), 'states',
            ArrayOf(chlk.models.bgtasks.BgTaskType), 'types',
            ArrayOf(chlk.models.district.District), 'districts',

            Number, 'typeId',
            Number, 'stateId',
            chlk.models.id.DistrictId, 'districtId',
            Boolean, 'allDistricts',


            [[chlk.models.common.PaginatedList,
               ArrayOf(chlk.models.district.District),
               ArrayOf(chlk.models.bgtasks.BgTaskState),
               ArrayOf(chlk.models.bgtasks.BgTaskType),
                Number,
                Number,
                chlk.models.id.DistrictId,
                Boolean
            ]],
                function $(bgTasks_, districts_, states_, types_, stateId_,  typeId_,  districtId_, allDistrictds_){
                    BASE();
                    if(bgTasks_)
                        this.setBgTasks(bgTasks_);
                    if(states_)
                        this.setStates(states_);
                    if(types_)
                        this.setTypes(types_);
                    if(districts_)
                        this.setDistricts(districts_);
                    if(typeId_)
                        this.setTypeId(typeId_);
                    if(stateId_)
                        this.setStateId(stateId_);
                    if(districtId_)
                        this.setDistrictId(districtId_);
                    this.setAllDistricts(!!allDistrictds_);
            }
        ]);
});
