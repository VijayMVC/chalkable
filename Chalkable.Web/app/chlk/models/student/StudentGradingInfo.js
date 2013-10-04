REQUIRE('chlk.models.people.ShortUserInfo');
REQUIRE('chlk.models.student.ClassPersonGradingInfo');

NAMESPACE('chlk.models.student', function(){
    "use strict";
    /**@class chlk.models.student.StudentGradingInfo*/

    CLASS('StudentGradingInfo', EXTENDS(chlk.models.people.ShortUserInfo),[

        [ria.serialize.SerializeProperty('studentgradings')],
        ArrayOf(chlk.models.student.ClassPersonGradingInfo), 'studentGradings'

    ]);
});