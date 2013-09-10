REQUIRE('chlk.models.people.UsersList');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.people.UsersListSubmit');

NAMESPACE('chlk.models.teacher', function () {
    "use strict";

    /** @class chlk.models.teacher.StudentsList*/
    CLASS(
        'StudentsList', EXTENDS(chlk.models.people.UsersListSubmit), [
            chlk.models.people.UsersList, 'usersPart', //todo: rename
            chlk.models.classes.ClassesForTopBar, 'topData',//todo: rename
            Boolean, 'my',
            chlk.models.id.ClassId, 'classId'
        ]);
});
