REQUIRE('chlk.models.common.PageWithClasses');
REQUIRE('chlk.models.id.GradingPeriodId');

NAMESPACE('chlk.models.common', function () {
    "use strict";

    /** @class chlk.models.common.PageWithClassesAndGradingPeriodsViewData*/
    CLASS(
        'PageWithClassesAndGradingPeriodsViewData', EXTENDS(chlk.models.common.PageWithClasses), [
            chlk.models.id.GradingPeriodId, 'gradingPeriodId',

            [[chlk.models.id.GradingPeriodId, chlk.models.classes.ClassesForTopBar, chlk.models.id.ClassId]],
            function $(gradingPeriodId_, topData_, selectedId_){
                BASE(topData_, selectedId_);
                if(gradingPeriodId_)
                    this.setGradingPeriodId(gradingPeriodId_);
            }
        ]);
});
