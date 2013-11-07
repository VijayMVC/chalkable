
NAMESPACE('chlk.models.api', function () {
    "use strict";

    /** @class chlk.models.api.ApiListItem*/
    CLASS(
        'ApiListItem', [
            String, 'name',

            String, 'role',
            Number, 'index',
            Boolean, 'separator',

            [ria.serialize.SerializeProperty('requiredparams')],
            ArrayOf(String), 'requiredParams',

            [ria.serialize.SerializeProperty('ismethod')],
            Boolean, 'method'

        ])
});
