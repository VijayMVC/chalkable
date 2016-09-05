REQUIRE('chlk.models.common.BaseCopyImportViewData');
REQUIRE('ria.serialize.SJX');

NAMESPACE('chlk.models.announcement.post', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.announcement.post.AnnouncementImportPostViewData*/
    CLASS(
        UNSAFE, 'AnnouncementImportPostViewData', EXTENDS(chlk.models.common.BaseCopyImportViewData), IMPLEMENTS(ria.serialize.IDeserializable), [

            OVERRIDE, VOID, function deserialize(raw) {
                BASE(raw);
                this.selectedAnnouncements = SJX.fromValue(raw.selectedAnnouncements, String);
            },

            String, 'selectedAnnouncements'
        ]);
});
