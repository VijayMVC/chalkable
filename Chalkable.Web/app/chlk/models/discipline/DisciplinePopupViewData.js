REQUIRE('chlk.models.Popup');
REQUIRE('chlk.models.discipline.DisciplineList');

NAMESPACE('chlk.models.discipline', function(){
   "use strict";

    /**@class chlk.models.discipline.DisciplinePopupViewData*/
    CLASS('DisciplinePopupViewData', EXTENDS(chlk.models.Popup),[

        chlk.models.discipline.DisciplineList, 'disciplineList',

        String, 'controller',
        String, 'action',
        String, 'params',

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