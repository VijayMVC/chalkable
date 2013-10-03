REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');

REQUIRE('chlk.models.settings.Preference');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.PreferenceService*/
    CLASS(
        'PreferenceService', EXTENDS(chlk.services.BaseService), [
            [[String]],
            ria.async.Future, function getPublic(key) {
                return this.get('Preference/GetPublic.json', chlk.models.settings.Preference, {
                    key: key
                });
            }
        ])
});