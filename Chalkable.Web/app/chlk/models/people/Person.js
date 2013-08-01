REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.people.User');

NAMESPACE('chlk.models.people', function () {
    "use strict";

    /** @class chlk.models.people.Person*/
    CLASS(
        'Person', EXTENDS(chlk.models.people.User), [
            chlk.models.common.ChlkDate, 'birthDate',
            String, 'salutation'
        ]);
});
