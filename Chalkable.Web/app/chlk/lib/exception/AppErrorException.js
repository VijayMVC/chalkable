NAMESPACE('chlk.lib.exception', function () {
    "use strict";

    /** @class chlk.lib.exception.AppErrorException */
    EXCEPTION(
        'AppErrorException', [
            function $(e_) {
                BASE('Application error exception', e_);
            }
        ]);
});