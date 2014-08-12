NAMESPACE('chlk.models.grading', function () {
    "use strict";

    /** @class chlk.models.grading.UserForReport*/
    CLASS(
        'UserForReport', [

            [ria.serialize.SerializeProperty('displayname')],
            String, 'displayName',

            String, 'comment',

            String, 'gender',

            chlk.models.id.SchoolPersonId, 'id',

            String, 'pictureUrl',

            [ria.serialize.SerializeProperty('role')],
            chlk.models.people.Role, 'role'
        ]);
});
