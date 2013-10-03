REQUIRE('chlk.models.people.User');

NAMESPACE('chlk.models.student', function(){
    "use strict";

    /**@class chlk.models.student.StudentInfo*/

    CLASS('StudentInfo', EXTENDS(chlk.models.people.User),[

        ArrayOf(chlk.models.people.User), 'parents'
    ]);
});