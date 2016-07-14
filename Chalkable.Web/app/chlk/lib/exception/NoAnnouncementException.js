NAMESPACE('chlk.lib.exception', function () {
    "use strict";

    /** @class chlk.lib.exception.NoAnnouncementException */
    EXCEPTION(
        'NoAnnouncementException', [
            function $(msg_, e_) {
                BASE(msg_ || 'Announcement is not found', e_);
            }
        ]);
});