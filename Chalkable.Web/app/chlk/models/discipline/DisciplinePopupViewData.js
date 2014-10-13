REQUIRE('chlk.models.common.AttendanceDisciplinePopUp');
REQUIRE('chlk.models.discipline.Discipline');
REQUIRE('chlk.models.discipline.DisciplineType');
REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.models.discipline', function(){
   "use strict";

    /**@class chlk.models.discipline.DisciplinePopupViewData*/
    CLASS('DisciplinePopupViewData', EXTENDS(chlk.models.common.AttendanceDisciplinePopUp),[
        
        ArrayOf(chlk.models.discipline.Discipline), 'disciplines',

        ArrayOf(chlk.models.discipline.DisciplineType), 'disciplineTypes',

        chlk.models.common.ChlkDate, 'date',

        [[ria.dom.Dom, ria.dom.Dom, String, String, String]],
        function $(target_, container_, controller_, action_, params_){
            BASE(target_, container_);
            if(controller_)
                this.setController(controller_);
            if(action_)
                this.setAction(action_);
            if(params_)
                this.setParams(params_);
        }

    ]);
});