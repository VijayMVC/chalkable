REQUIRE('ria.serialize.SJX');
REQUIRE('ria.serialize.IDeserializable');
/**
 * Created with JetBrains WebStorm.
 * User: Yuran
 * Date: 5/28/14
 * Time: 3:14 AM
 * To change this template use File | Settings | File Templates.
 */

NAMESPACE('chlk.models.school', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.school.SchoolOption*/
    CLASS(
        UNSAFE, FINAL,  'SchoolOption', IMPLEMENTS(ria.serialize.IDeserializable), [
        VOID, function deserialize(raw) {
            this.allowScoreEntryForUnexcused = SJX.fromValue(raw.allowscoreentryforunexcused, Boolean);
        },
        Boolean, 'allowScoreEntryForUnexcused',

        [[Boolean]],
        function $fromRaw(allowScoreEntryForUnexcused) {
            BASE();
            this.allowScoreEntryForUnexcused = allowScoreEntryForUnexcused;
        }
    ]);
});
