NAMESPACE('chlk.lib.exception', function () {
    "use strict";

    /** @class chlk.lib.exception.AnnouncementDeleteFailedException */
    EXCEPTION(
        'AnnouncementDeleteFailedException', [
            function $(msg_, e_, title_) {
                BASE(msg_ || 'Announcement deletions failed.', e_);
                this.title = title_;
            },
            String, 'title'
        ]);
});