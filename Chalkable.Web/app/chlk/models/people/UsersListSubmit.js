NAMESPACE('chlk.models.people', function () {
    "use strict";

    /** @class chlk.models.people.UsersListSubmit*/
    CLASS(
        'UsersListSubmit', [
            Number, 'selectedIndex',
            Boolean, 'byLastName',
            String, 'filter',
            Number, 'start',
            String, 'submitType'
        ]);
});
