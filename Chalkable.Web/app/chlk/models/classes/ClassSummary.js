REQUIRE('chlk.models.classes.Class');
REQUIRE('chlk.models.feed.Feed');
REQUIRE('chlk.models.classes.Room');

NAMESPACE('chlk.models.classes', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.classes.ClassSummary*/
    CLASS(
        'ClassSummary', EXTENDS(chlk.models.classes.Class), [
            chlk.models.classes.Room, 'room',

            chlk.models.feed.Feed, 'feed',

            OVERRIDE, VOID, function deserialize(raw){
                BASE(raw);
                this.room = SJX.fromDeserializable(raw.room, chlk.models.classes.Room);
            }
        ]);
});
