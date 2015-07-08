REQUIRE('chlk.models.schoolYear.Year');

NAMESPACE('chlk.models.schoolYear', function () {
    "use strict";
    /** @class chlk.models.schoolYear.Years*/
    CLASS(
        'Years', [
            ArrayOf(chlk.models.schoolYear.Year), 'items'
        ]);
});
