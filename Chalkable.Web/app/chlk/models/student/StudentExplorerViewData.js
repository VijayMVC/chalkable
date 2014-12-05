REQUIRE('chlk.models.people.Schedule');
REQUIRE('chlk.models.calendar.announcement.Day');
REQUIRE('chlk.models.people.UserProfileViewData');
REQUIRE('chlk.models.calendar.BaseCalendar');

NAMESPACE('chlk.models.student', function () {
    "use strict";

    /** @class chlk.models.student.StudentExplorerViewData*/
    CLASS(
        'StudentExplorerViewData', EXTENDS(chlk.models.people.UserProfileViewData.OF(chlk.models.student.StudentExplorer)), [

            chlk.models.student.StudentExplorer, 'studentExplorer',

            chlk.models.people.Schedule, function getStudentExplorer(){return this.getUser();},
            [[chlk.models.student.StudentExplorer]],
            VOID, function setStudentExplorer(studentExplorer){return this.setUser_(studentExplorer);},

            [[
                chlk.models.common.Role,
                chlk.models.student.StudentExplorer,
                ArrayOf(chlk.models.people.Claim),
            ]],
            function $(role, studentExplorer_, claims_){
                BASE(role, studentExplorer_, claims_);
            }
        ]);
});
