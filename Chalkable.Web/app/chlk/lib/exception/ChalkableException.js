NAMESPACE('chlk.lib.exception', function () {
    "use strict";

    /** @class chlk.lib.exception.ChalkableException */
    EXCEPTION(
        'ChalkableException', [
            function $(msg_, e_, title_) {
                BASE(msg_ || 'Chalkable exception', e_);
                this.title = title_;
            },
            String, 'title'
        ]);
});