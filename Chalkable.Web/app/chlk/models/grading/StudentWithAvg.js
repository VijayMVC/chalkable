NAMESPACE('chlk.models.grading', function () {
    "use strict";
    /** @class chlk.models.grading.StudentWithAvg*/
    CLASS(
        'StudentWithAvg', [
            [ria.serialize.SerializeProperty('studentinfo')],
            chlk.models.people.ShortUserInfo, 'studentInfo',
            Number, 'avg'
        ]);
});
