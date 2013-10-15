REQUIRE('chlk.models.student.StudentGradesHoverBoxItem');
REQUIRE('chlk.models.common.HoverBox');

NAMESPACE('chlk.models.student', function () {
    "use strict";

    /** @class chlk.models.student.StudentGradesHoverBox*/
    CLASS(
        'StudentGradesHoverBox', EXTENDS(chlk.models.common.HoverBox), [
            ArrayOf(chlk.models.student.StudentGradesHoverBoxItem), 'hover'
        ]);
});
