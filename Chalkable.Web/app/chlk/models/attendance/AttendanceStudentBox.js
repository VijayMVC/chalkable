REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.attendance.AttendanceStudentBox*/
    CLASS(
        'AttendanceStudentBox', [
            chlk.models.id.SchoolPersonId, 'id',

            Object, 'student',

            chlk.models.common.ChlkDate, 'date',

            Number, 'top',

            Number, 'currentPage',

            String, 'gradeLevelsIds',

            [[Object, chlk.models.common.ChlkDate, Number, Number, String]],
            function $(student_, date_, top_, currentPage_, gradeLevelsIds_){
                BASE();
                if(top_)
                    this.setTop(top_);
                if(student_)
                    this.setStudent(student_);
                if(date_)
                    this.setDate(date_);
                if(currentPage_)
                    this.setCurrentPage(currentPage_);
                if(gradeLevelsIds_)
                    this.setGradeLevelsIds(gradeLevelsIds_);
            }
        ]);
});
