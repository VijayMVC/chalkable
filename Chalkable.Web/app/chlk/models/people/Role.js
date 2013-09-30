REQUIRE('chlk.models.id.SchoolPersonId');
NAMESPACE('chlk.models.people', function () {
    "use strict";


    //todo: join with chlk.models.common.role
    /** @class chlk.models.people.Role*/
    CLASS(
        'Role', [
            String, 'description',

            Number, 'id',

            String, 'name',

            [ria.serialize.SerializeProperty('namelowered')],
            String, 'nameLowered'
        ]);
});
