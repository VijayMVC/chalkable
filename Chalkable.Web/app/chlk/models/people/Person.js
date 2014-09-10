REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.people.Phone');

NAMESPACE('chlk.models.people', function () {
    "use strict";

    /** @class chlk.models.people.Person*/
    CLASS(
        'Person', EXTENDS(chlk.models.people.User), [

        ]);
});
