NAMESPACE('chlk.lib.exception', function () {
    "use strict";

    /** @class chlk.lib.exception.NoClassAnnouncementTypeException */
    EXCEPTION(
        'NoClassAnnouncementTypeException', [
            function $(e_) {
                BASE('No Class Announcement Type Exception', e_);
            }
        ]);
});