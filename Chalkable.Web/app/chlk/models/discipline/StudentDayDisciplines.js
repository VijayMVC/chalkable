REQUIRE('chlk.models.calendar.discipline.StudentDisciplineCalendarMonthItem');
REQUIRE('chlk.models.Popup');

NAMESPACE('chlk.models.discipline', function(){
    "use strict";

    /** @class chlk.models.discipline.StudentDayDisciplines*/
    CLASS('StudentDayDisciplines', EXTENDS(chlk.models.Popup), [
        chlk.models.calendar.discipline.StudentDisciplineCalendarMonthItem, 'item',

        Boolean, 'canSetDiscipline',

        String, 'controller',

        String, 'action',

        String, 'params',

        [[ria.dom.Dom, chlk.models.calendar.discipline.StudentDisciplineCalendarMonthItem, Boolean, String, String, String]],
            function $(target_, item_, canSetDiscipline_, controller_, action_, params_){
                BASE(target_);
                if(item_)
                    this.setItem(item_);
                if(canSetDiscipline_)
                    this.setCanSetDiscipline(canSetDiscipline_);
                if(controller_)
                    this.setController(controller_);
                if(action_)
                    this.setAction(action_);
                if(params_)
                    this.setParams(params_);
            }
    ]);
});