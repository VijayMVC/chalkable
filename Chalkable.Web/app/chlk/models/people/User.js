NAMESPACE('chlk.models.people', function () {
    "use strict";
    /** @class chlk.models.people.User*/
    CLASS(
        'User', [
            Boolean, 'active',
            String, 'displayname',
            String, 'email',
            String, 'firstname',
            String, 'fullname',
            String, 'gender',
            String, 'grade',
            Number, 'id',
            String, 'lastname',
            String, 'localid',
            String, 'roledescription',
            String, 'rolename',
            Number, 'schoolid'
        ]);
});
