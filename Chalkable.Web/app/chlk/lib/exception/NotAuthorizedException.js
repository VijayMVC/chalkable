NAMESPACE('chlk.lib.exception', function () {
    "use strict";

    /** @class chlk.lib.exception.NotAuthorizedException */
    EXCEPTION(
        'NotAuthorizedException', [
            function $(e_) {
                BASE('Not authorized exception', e_);
            }
        ]);
});