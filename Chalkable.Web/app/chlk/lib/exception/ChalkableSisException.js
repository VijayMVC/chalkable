NAMESPACE('chlk.lib.exception', function () {
    "use strict";

    /** @class chlk.lib.exception.ChalkableSisException */
    EXCEPTION(
        'ChalkableSisException', [
            function $(msg_, e_) {
                BASE(msg_ || 'Chalkable SIS exception', e_);
            }
        ]);
});