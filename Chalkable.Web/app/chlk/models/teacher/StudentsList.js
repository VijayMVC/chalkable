REQUIRE('chlk.models.people.UsersList');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.people.UsersListSubmit');

NAMESPACE('chlk.models.teacher', function () {
    "use strict";

    /** @class chlk.models.teacher.StudentsList*/
    CLASS(
        'StudentsList', EXTENDS(chlk.models.people.UsersListSubmit), [
            chlk.models.people.UsersList, 'usersList', //todo: rename
            chlk.models.classes.ClassesForTopBar, 'topData',//todo: rename
            Boolean, 'my',

            [[chlk.models.people.UsersList, chlk.models.classes.ClassesForTopBar, Boolean, chlk.models.id.ClassId]],
            function $(usersList_, classes_, isMy_, classId_){
                BASE();
                if(usersList_)
                    this.setUsersList(usersList_);
                if(classes_)
                    this.setTopData(classes_);
                this.setMy(isMy_ || false);
                if(classId_)
                    this.setClassId(classId_);
            }
        ]);
});
