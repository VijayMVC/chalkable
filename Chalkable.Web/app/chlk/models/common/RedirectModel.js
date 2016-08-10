NAMESPACE('chlk.models.common', function () {
    "use strict";

    /** @class chlk.models.common.RedirectModel */
    CLASS('RedirectModel',[
        [ria.serialize.SerializeProperty('redirecturl')],
        String, 'redirectUrl'
    ]);
});