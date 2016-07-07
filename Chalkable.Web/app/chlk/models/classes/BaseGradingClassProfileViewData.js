REQUIRE('chlk.models.classes.BaseClassProfileViewData');

NAMESPACE('chlk.models.classes', function(){
   "use strict";

    /** @class chlk.models.classes.GradingPageTypeEnum*/
    ENUM('GradingPageTypeEnum', {
        ITEMS_BOXES: 0,
        ITEMS_GRID: 1,
        STANDARDS_BOXES: 2,
        STANDARDS_GRID: 3,
        FINAL_GRADES: 4
    });

    /**@class chlk.models.classes.BaseGradingClassProfileViewData*/
    CLASS('BaseGradingClassProfileViewData', EXTENDS(chlk.models.classes.BaseClassProfileViewData),[

        chlk.models.classes.GradingPageTypeEnum, 'gradingPageType',

        [[chlk.models.common.Role, chlk.models.classes.Class, ArrayOf(chlk.models.people.Claim), Boolean, chlk.models.classes.GradingPageTypeEnum]],
        function $(role_, clazz_, claims_, isAssignedToClass_, gradingPageType_){
            BASE(role_, clazz_, claims_, isAssignedToClass_);
            if(gradingPageType_)
                this.setGradingPageType(gradingPageType_);
        }
    ])
});