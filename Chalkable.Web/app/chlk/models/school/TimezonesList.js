REQUIRE('chlk.models.school.Timezone');

NAMESPACE('chlk.models.school', function () {
    "use strict";

    /** @class chlk.models.school.TimezonesList*/
    CLASS(
        'TimezonesList', [
            ArrayOf(chlk.models.school.Timezone), 'items'
        ]);
});