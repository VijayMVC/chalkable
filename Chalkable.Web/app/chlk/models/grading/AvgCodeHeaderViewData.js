REQUIRE('chlk.models.grading.AvgComment');
REQUIRE('chlk.models.id.CodeHeaderId');

NAMESPACE('chlk.models.grading', function () {
    "use strict";
    /** @class chlk.models.grading.AvgCodeHeaderViewData*/
    CLASS(
        'AvgCodeHeaderViewData', [
            [ria.serialize.SerializeProperty('headerid')],
            chlk.models.id.CodeHeaderId, 'headerId',

            [ria.serialize.SerializeProperty('headername')],
            String, 'headerName',

            [ria.serialize.SerializeProperty('gradingcomment')],
            chlk.models.grading.AvgComment, 'gradingComment'
        ]);
});