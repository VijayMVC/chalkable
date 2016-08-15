REQUIRE('chlk.models.people.User');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.attendance.AbsentNowStudent*/
    CLASS(
        'AbsentNowStudent', EXTENDS(chlk.models.people.User), [
            [ria.serialize.SerializeProperty('absentscount')],
            Number, 'absentsCount'
        ]);
});
