NAMESPACE('chlk.models.settings', function () {
    "use strict";
    /** @class chlk.models.settings.Preference*/
    CLASS(
        'Preference', [
            String, 'key',
            String, 'value',
            [ria.serialize.SerializeProperty('ispublicpref')],
            Boolean, 'publicPreference',
            Number, 'category',
            Number, 'type',
            String, 'hint'
        ]);
});
