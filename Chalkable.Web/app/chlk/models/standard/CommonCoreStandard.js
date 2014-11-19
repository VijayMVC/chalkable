NAMESPACE('chlk.models.standard', function () {
    "use strict";
    /** @class chlk.models.standard.CommonCoreStandard*/
    CLASS(
        'CommonCoreStandard', [

            [ria.serialize.SerializeProperty('standardcode')],
            String, 'standardCode',

            [ria.serialize.SerializeProperty('description')],
            String, 'description'
    ]);
});
