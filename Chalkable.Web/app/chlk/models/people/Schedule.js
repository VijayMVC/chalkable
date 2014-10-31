REQUIRE('chlk.models.people.User');
NAMESPACE('chlk.models.people', function () {
    "use strict";

    /** @class chlk.models.people.Schedule*/
    CLASS(
        'Schedule', EXTENDS(chlk.models.people.User), [
            [ria.serialize.SerializeProperty('classesnumber')],
            Number, 'classesNumber'
        ]);
});
