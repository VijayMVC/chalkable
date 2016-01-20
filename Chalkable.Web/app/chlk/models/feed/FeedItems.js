REQUIRE('chlk.models.common.PageWithClasses');
REQUIRE('chlk.models.announcement.FeedAnnouncementViewData');
REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.models.feed', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.feed.FeedItems*/
    CLASS(
        'FeedItems',
                IMPLEMENTS(ria.serialize.IDeserializable), [

            VOID, function deserialize(raw) {
                this.items = SJX.fromArrayOfDeserializables(raw.items, chlk.models.announcement.FeedAnnouncementViewData);
            },

            [[ArrayOf(chlk.models.announcement.FeedAnnouncementViewData)]],
            function $(items_){
                BASE();
                if(items_)
                    this.setItems(items_);
            },

            ArrayOf(chlk.models.announcement.FeedAnnouncementViewData), 'items',

            Boolean, 'readonly'
        ]);
});