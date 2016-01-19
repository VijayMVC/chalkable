REQUIRE('ria.serialize.SJX');
REQUIRE('ria.serialize.IDeserializable');

REQUIRE('chlk.models.apps.AppMarketApplication');

NAMESPACE('chlk.models.apps', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.apps.ApplicationAuthorization */
    CLASS(
        UNSAFE,  'ApplicationAuthorization', IMPLEMENTS(ria.serialize.IDeserializable), [
            VOID, function deserialize(raw){
                this.authorizationCode = SJX.fromValue(raw.authorizationcode, String);
                this.application = SJX.fromDeserializable(raw.applicationinfo, chlk.models.apps.AppMarketApplication);
            },

            String, 'authorizationCode',
            chlk.models.apps.AppMarketApplication, 'application'
        ]);
});
