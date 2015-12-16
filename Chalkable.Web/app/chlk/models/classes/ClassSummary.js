REQUIRE('chlk.models.classes.Class');
REQUIRE('chlk.models.feed.Feed');
REQUIRE('chlk.models.classes.Room');

NAMESPACE('chlk.models.classes', function () {
    "use strict";

    /** @class chlk.models.classes.ClassSummary*/
    CLASS(
        'ClassSummary', EXTENDS(chlk.models.classes.Class), [
            chlk.models.classes.Room, 'room',

            chlk.models.feed.Feed, 'feed'
        ]);
});
