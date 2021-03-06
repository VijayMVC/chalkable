REQUIRE('chlk.models.classes.BaseClassProfileViewData');
REQUIRE('chlk.models.classes.ClassGradingViewData');

NAMESPACE('chlk.models.classes', function(){
    "use strict";
    /**@class chlk.models.classes.ClassProfileGradingViewData*/
    CLASS('ClassProfileGradingViewData', EXTENDS(chlk.models.classes.BaseClassProfileViewData), [

        chlk.models.classes.ClassGradingViewData, 'gradingPart',

        [[chlk.models.common.Role, chlk.models.classes.ClassGradingViewData, ArrayOf(chlk.models.people.Claim), Boolean]],
        function $(role_, gradingPart_, claims_, isAssignedToClass_){
            BASE(role_, gradingPart_, claims_, isAssignedToClass_);
            if(gradingPart_)
                this.setGradingPart(gradingPart_);
        }
    ]);
});