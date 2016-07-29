NAMESPACE('chlk.lib.exception', function () {
    "use strict";

    /** @class chlk.lib.exception.FileSizeExceedException */
    EXCEPTION(
        'FileSizeExceedException', [
            function $(msg_, e_) {
                BASE(msg_ || 'File size exceeds exception', e_);
            }
        ]);
});