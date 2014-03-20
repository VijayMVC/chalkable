NAMESPACE('chlk.models.grading', function () {
    "use strict";
    /** @class chlk.models.grading.AlertsEnum*/
    ENUM(
        'AlertsEnum', {
            DROP: 1,
            LATE: 2,
            INCOMPLETE: 3,
            EXEMPT: 4,
            ABSENT: 5,
            FILL: 6
        });
});
