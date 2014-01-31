REQUIRE('chlk.models.people.User');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.attendance.AdminStudentsBox*/
    CLASS(
        'AdminStudentsBox', [
            ArrayOf(chlk.models.people.User), 'students',

            [[ArrayOf(chlk.models.people.User)]],
            function $(students_){
                BASE();
                if(students_)
                    this.setStudents(students_);
            }
        ]);
});
