REQUIRE('chlk.models.common.PaginatedList');
NAMESPACE('chlk.models.people', function () {
    "use strict";

    /** @class chlk.models.people.UsersList*/
    CLASS(
        'UsersList', [
            chlk.models.common.PaginatedList, 'users',
            Number, 'selectedIndex',
            Boolean, 'byLastName',
            String, 'filter',
            Number, 'start'
        ]);
});
