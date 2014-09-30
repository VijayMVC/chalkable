NAMESPACE('chlk.models.grading', function () {
    "use strict";
    /** @class chlk.models.grading.StudentWithAvg*/
    CLASS(
        'StudentWithAvg', [
            [ria.serialize.SerializeProperty('studentinfo')],
            chlk.models.people.ShortUserInfo, 'studentInfo',

            [ria.serialize.SerializeProperty('iswithdrawn')],
            Boolean, 'withdrawn',

            Number, 'avg'
        ]);
});
