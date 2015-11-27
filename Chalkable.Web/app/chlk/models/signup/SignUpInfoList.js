REQUIRE('chlk.models.signup.SignUpInfo');

NAMESPACE('chlk.models.signup', function () {
    "use strict";

    /** @class chlk.models.signup.SignUpInfoList*/
    CLASS(
        'SignUpInfoList', [
            ArrayOf(chlk.models.signup.SignUpInfo), 'items'
        ]);
});