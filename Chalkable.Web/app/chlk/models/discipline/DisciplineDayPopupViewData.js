REQUIRE('chlk.models.common.AttendanceDisciplinePopUp');
REQUIRE('chlk.models.discipline.DisciplineList');

NAMESPACE('chlk.models.discipline', function(){
   "use strict";

    /**@class chlk.models.discipline.DisciplineDayPopupViewData*/
    CLASS('DisciplineDayPopupViewData', EXTENDS(chlk.models.common.AttendanceDisciplinePopUp),[

        [ria.serialize.SerializeProperty('disciplinelist')],
        chlk.models.discipline.DisciplineList, 'disciplineList',

        [[ria.dom.Dom, ria.dom.Dom, chlk.models.discipline.DisciplineList
            , String, String, String
        ]],
        function $(target_, container_, disciplineList_, controller_, action_, params_){
            BASE(target_, container_);
            if(disciplineList_)
                this.setDisciplineList(disciplineList_);
            if(controller_)
                this.setController(controller_);
            if(action_)
                this.setAction(action_);
            if(params_)
                this.setParams(params_);
        }

    ]);
});