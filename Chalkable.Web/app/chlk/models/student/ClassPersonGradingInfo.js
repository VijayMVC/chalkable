REQUIRE('chlk.models.id.ClassPersonId');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.id.CourseId');


NAMESPACE('chlk.models.student', function (){
   "use strict";

    /**@class chlk.models.student.ClassPersonGradingInfo*/
    CLASS('ClassPersonGradingInfo',  [

        [ria.serialize.SerializeProperty('classpersonid')],
        chlk.models.id.ClassPersonId, 'classPersonId',

        [ria.serialize.SerializeProperty('classid')],
        chlk.models.id.ClassId, 'classId',

        [ria.serialize.SerializeProperty('personid')],
        chlk.models.id.SchoolPersonId, 'personId',

        [ria.serialize.SerializeProperty('classname')],
        String, 'className',

        [ria.serialize.SerializeProperty('courseid')],
        chlk.models.id.CourseId, 'courseId',

        [ria.serialize.SerializeProperty('classavg')],
        Number, 'classAvg',

        [ria.serialize.SerializeProperty('studentavg')],
        Number, 'studentAvg'
    ]);
});