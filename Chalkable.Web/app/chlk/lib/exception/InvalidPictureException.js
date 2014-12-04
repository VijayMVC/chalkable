NAMESPACE('chlk.lib.exception', function () {
    "use strict";

    /** @class chlk.lib.exception.InvalidPictureException */
    EXCEPTION(
        'InvalidPictureException', [
            function $(e_) {
                BASE('Invalid picture exception', e_);
            }
        ]);
});