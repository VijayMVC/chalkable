REQUIRE('chlk.models.Popup');

NAMESPACE('chlk.models.school', function () {
    "use strict";
    /** @class chlk.models.school.ActionButtons*/
    CLASS(
        'ActionButtons', EXTENDS(chlk.models.Popup), [
            ArrayOf(Number), 'buttons',
            ArrayOf(String), 'emails'
        ]);
});
