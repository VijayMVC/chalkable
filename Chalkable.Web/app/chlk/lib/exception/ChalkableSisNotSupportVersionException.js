NAMESPACE('chlk.lib.exception', function () {
    "use strict";

    /** @class chlk.lib.exception.ChalkableSisNotSupportVersionException */
    EXCEPTION(
        'ChalkableSisNotSupportVersionException', [
            function $(msg_, e_) {
                BASE(msg_ || 'Chalkable SIS not support version exception', e_);
            }
        ]);
});