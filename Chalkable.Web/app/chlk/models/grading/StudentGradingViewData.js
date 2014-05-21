REQUIRE('chlk.models.people.User');

NAMESPACE('chlk.models.grading', function () {
    "use strict";

    /** @class chlk.models.grading.StudentWellTroubleEnum*/
    ENUM('StudentWellTroubleEnum', {
        NORMALL: 0,
        WELL: 1,
        TROUBLE: 2
    });

    /** @class chlk.models.grading.StudentGradingViewData*/
    CLASS(
        'StudentGradingViewData', EXTENDS(chlk.models.people.User), [
            Number, 'avg',
            Number, 'right',
            chlk.models.grading.StudentWellTroubleEnum, 'wellTroubleType'
        ]);
});
