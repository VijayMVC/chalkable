REQUIRE('chlk.models.settings.Preference');
NAMESPACE('chlk.models.settings', function () {
    "use strict";
    /** @class chlk.models.settings.PreferencesList*/
    CLASS(
        'PreferencesList', [
            ArrayOf(chlk.models.settings.Preference), 'items',

            [[ArrayOf(chlk.models.settings.Preference)]],
            function $(items){
                this.setItems(items);
            }
        ]);
});
