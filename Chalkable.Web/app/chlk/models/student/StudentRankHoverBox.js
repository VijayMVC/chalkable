REQUIRE('chlk.models.student.StudentRankHoverBoxItem');
REQUIRE('chlk.models.common.HoverBox');

NAMESPACE('chlk.models.student', function () {
    "use strict";

    /** @class chlk.models.student.StudentRankHoverBox*/
    CLASS(
        'StudentRankHoverBox', EXTENDS(chlk.models.common.HoverBox), [
            ArrayOf(chlk.models.student.StudentRankHoverBoxItem), 'hover'
        ]);
});
