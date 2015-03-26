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
            },

            [[chlk.models.settings.PreferenceCategoryEnum]],
            ria.async.Future, function getPreferences(category_) {
                return this.getArray('Preference/List.json', chlk.models.settings.Preference, {
                    category: category_ && category_.valueOf()
                });
            },

            [[String, String, Boolean]],
            ria.async.Future, function setPreference(key, value, isPublic){
                return this.postArray('Preference/Set.json', chlk.models.settings.Preference, {
                    key: key,
                    value: value,
                    ispublic: isPublic
                });
            },

            ArrayOf(chlk.models.settings.PreferenceCategory), function getPreferencesCategories(){
                var res = [
                    new chlk.models.settings.PreferenceCategory(chlk.models.settings.PreferenceCategoryEnum.COMMON),
                    new chlk.models.settings.PreferenceCategory(chlk.models.settings.PreferenceCategoryEnum.CONTROLLER_DESCRIPTIONS),
                    new chlk.models.settings.PreferenceCategory(chlk.models.settings.PreferenceCategoryEnum.EMAIL_TEXT)
                ];
                return res;
            },
        ]);
});