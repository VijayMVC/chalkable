REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.settings.Preference');
REQUIRE('chlk.models.settings.PreferencesList');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.SettingsService */
    CLASS(
        'SettingsService', EXTENDS(chlk.services.BaseService), [

            [[Number]],
            ria.async.Future, function getPreferences(category_) {
                return this.getArray('Preference/List.json', chlk.models.settings.Preference, {
                    category:category_
                }).then(function(preferences){
                    return new ria.async.DeferredData(new chlk.models.settings.PreferencesList(preferences));
                });
            },

            [[String, String, Boolean]],
            ria.async.Future, function setPreference(key, value, isPublic){
                return this.postArray('Preference/Set.json', chlk.models.settings.Preference, {
                    key: key,
                    value: value,
                    ispublic: isPublic
                }).then(function(preferences){
                    return new ria.async.DeferredData(new chlk.models.settings.PreferencesList(preferences));
                });
            }

        ])
});