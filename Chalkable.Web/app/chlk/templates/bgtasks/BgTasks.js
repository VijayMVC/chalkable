REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.bgtasks.BgTasksListViewData');

NAMESPACE('chlk.templates.bgtasks', function () {

    /** @class chlk.templates.bgtasks.BgTasks*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/bgtasks/BgTasks.jade')],
        [ria.templates.ModelBind(chlk.models.bgtasks.BgTasksListViewData)],
        'BgTasks', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            chlk.models.common.PaginatedList, 'bgTasks',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.bgtasks.BgTaskState), 'states',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.bgtasks.BgTaskType), 'types',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.district.District), 'districts',

            [ria.templates.ModelPropertyBind],
            Number, 'typeId',

            [ria.templates.ModelPropertyBind],
            Number, 'stateId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.DistrictId, 'districtId',

            [ria.templates.ModelPropertyBind],
            Boolean, 'allDistricts',


            String, function getTasksIds(){
                var tasksIds = this.getBgTasks().getItems().map(function(item){return item.getId().valueOf();});
                return tasksIds.join(',');
            },

            Array, function prepareDistrictsSelectModel(){
                var districtId = this.getDistrictId();
                var allDistricts = this.isAllDistricts();
                var res = [{
                    value: null,
                    displayValue: 'All',
                    selected: true,
                    allDistricts: true,
                },{
                    value: null,
                    displayValue: 'Global Task',
                    selected: !allDistricts && !districtId,
                    allDistricts: false,
                }];
                var districts = this.getDistricts();
                for(var i = 0; i < districts.length; i++){
                    var obj = {
                        value: districts[i].getId(),
                        displayValue: districts[i].getName(),
                        selected: false,
                        allDistricts: allDistricts,
                    };
                    if(districts[i].getId() == districtId){
                        res[0].selected = false;
                        obj.selected = true;
                    }
                    res.push(obj)
                }
                return res;
            },

            Array, function prepareStatesSelectModel(){
                var res = [{
                    value: null,
                    displayValue: 'All',
                    selected: true
                }];
                var states = this.getStates();
                for(var i = 0; i < states.length; i++){
                    var obj = {
                        value: states[i].getTypeId().valueOf(),
                        displayValue: states[i].toString(),
                        selected: false
                    };
                    if(states[i].getTypeId().valueOf() == this.getStateId()){
                        res[0].selected = false;
                        obj.selected = true;
                    }
                    res.push(obj)
                }
                return res;
            },

            Array, function prepareTypesSelectModel(){
                var res = [{
                    value: null,
                    displayValue: 'All',
                    selected: true
                }];
                var types = this.getTypes();
                for(var i = 0; i < types.length; i++){
                    var obj = {
                        value: types[i].getTypeId().valueOf(),
                        displayValue: types[i].toString(),
                        selected: false
                    };
                    if(types[i].getTypeId().valueOf() == this.getTypeId()){
                        res[0].selected = false;
                        obj.selected = true;
                    }
                    res.push(obj)
                }
                return res;
            }
        ])
});