REQUIRE('chlk.models.id.SchoolPersonId');

NAMESPACE('chlk.models.developer', function () {
    "use strict";
    /** @class chlk.models.developer.DeveloperBalance*/
    CLASS(
        'DeveloperBalance', [
            chlk.models.id.SchoolPersonId, 'id',
            Number, 'daysToPayout',
            Number, 'balance'
        ]);
});
