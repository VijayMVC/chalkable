NAMESPACE('chlk.lib.exception', function () {
    "use strict";

    /** @class chlk.lib.exception.NoAnnouncementException */
    EXCEPTION(
        'NoAnnouncementException', [
            function $(e_) {
                BASE('Announcement not found', e_);
            }
        ]);
});