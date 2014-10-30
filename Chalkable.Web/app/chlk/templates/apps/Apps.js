//REQUIRE('chlk.templates.PaginatedList');
REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.apps.Application');
REQUIRE('chlk.models.apps.AppsListViewData');

NAMESPACE('chlk.templates.apps', function () {

    /** @class chlk.templates.apps.Apps*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/apps.jade')],
        [ria.templates.ModelBind(chlk.models.apps.AppsListViewData)],
        'Apps', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            chlk.models.common.PaginatedList, 'applications',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.developer.DeveloperInfo), 'developers',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.apps.AppState), 'applicationStates',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.SchoolPersonId, 'developerId',

            [ria.templates.ModelPropertyBind],
            Number, 'stateId',

            Array, function prepareDevelopersSelectModel(){
                var res = [{
                    value: null,
                    displayValue: 'All',
                    selected: true
                }];
                var developers = this.getDevelopers();
                var currentDeveloperId = this.getDeveloperId();
                for(var i = 0; i < developers.length; i++){
                    var obj = {
                        value: developers[i].getId(),
                        displayValue: developers[i].getDisplayName(),
                        selected: false
                    };
                    if(developers[i].getId() == currentDeveloperId){
                        res[0].selected = true;
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
                var states = this.getApplicationStates();
                for(var i = 0; i < states.length; i++){
                    var obj = {
                        value: states[i].getStateId().valueOf(),
                        displayValue: states[i].getStatus(),
                        selected: false
                    };
                    if(states[i].getStateId().valueOf() == this.getStateId()){
                        res[0].selected = true;
                        obj.selected = true;
                    }
                    res.push(obj)
                }
                return res;
            }

        ])
});