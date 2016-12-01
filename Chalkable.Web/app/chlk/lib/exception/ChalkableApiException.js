NAMESPACE('chlk.lib.exception', function () {
    "use strict";

    /** @class chlk.lib.exception.ChalkableApiException */
    EXCEPTION(
        'ChalkableApiException', [
            function $(e_) {
                BASE('ChalkableApiException exception', e_);
            }
        ]);
});