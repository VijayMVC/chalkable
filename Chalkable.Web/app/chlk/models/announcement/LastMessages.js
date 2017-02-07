REQUIRE('chlk.models.common.Array');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.LastMessages*/
    CLASS(
        'LastMessages', EXTENDS(chlk.models.common.Array), [
            String, 'announcementTypeName',

            [[String, ArrayOf(String)]],
            function $create(announcementTypeName, msgs) {
                BASE();
                this.setAnnouncementTypeName(announcementTypeName);
                this.setItems(msgs);
            }
        ]);


});
