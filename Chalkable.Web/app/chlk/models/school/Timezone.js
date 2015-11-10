
NAMESPACE('chlk.models.school', function () {
    "use strict";
    /** @class chlk.models.school.Timezone*/
    CLASS(
        'Timezone', [
            String, 'id',
            [ria.serialize.SerializeProperty('displayname')],
            String, 'displayName',
            String, 'standartTime',
            String, 'daylightName',
            Boolean, 'supportsDaylightSavingTime'
        ]);
});
