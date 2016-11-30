NAMESPACE('chlk.lib.exception', function () {
    "use strict";

    /** @class chlk.lib.exception.DataException */
    EXCEPTION(
        'DataException', [
            function $(msg_, e_) {
                BASE(msg_ || 'Data request exception', e_);
            }
        ]);
});