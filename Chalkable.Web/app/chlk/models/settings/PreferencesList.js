REQUIRE('chlk.models.settings.Preference');
NAMESPACE('chlk.models.settings', function () {
    "use strict";
    /** @class chlk.models.settings.PreferencesList*/
    CLASS(
        'PreferencesList', [
            ArrayOf(chlk.models.settings.Preference), 'items',

            ArrayOf(chlk.models.settings.PreferenceCategory), 'preferenceCategories',

            chlk.models.settings.PreferenceCategory, 'currentCategory',

            [[ArrayOf(chlk.models.settings.Preference), ArrayOf(chlk.models.settings.PreferenceCategory),  chlk.models.settings.PreferenceCategory]],
            function $(items, preferenceCategories_, currentCategory_){
                BASE();
                this.setItems(items);
                if(preferenceCategories_)
                    this.setPreferenceCategories(preferenceCategories_);
                if(currentCategory_)
                    this.setCurrentCategory(currentCategory_)
            }
        ]);
});
