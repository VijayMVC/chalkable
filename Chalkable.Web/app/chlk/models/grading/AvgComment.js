REQUIRE('chlk.models.id.AvgCommentId');

NAMESPACE('chlk.models.grading', function () {
    "use strict";
    /** @class chlk.models.grading.AvgComment*/
    CLASS(
        'AvgComment', [
            String, 'code',

            String, 'comment',

            chlk.models.id.AvgCommentId, 'id',

            [ria.serialize.SerializeProperty('schoolref')],
            chlk.models.id.SchoolId, 'schoolId'
        ]);
});