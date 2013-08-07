NAMESPACE('chlk.models.common', function () {
    "use strict";
    /** @class chlk.models.common.InfoMsg*/
    CLASS(
        'InfoMsg', [
            String, 'text',
            Array, 'buttonsInfo'
        ]);
});
