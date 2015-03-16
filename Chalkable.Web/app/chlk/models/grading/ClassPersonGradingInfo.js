REQUIRE('chlk.models.id.ClassPersonId');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.id.CourseId');
REQUIRE('chlk.models.grading.ClassPersonGradingItem');

NAMESPACE('chlk.models.grading', function (){
   "use strict";

    /**@class chlk.models.grading.ClassPersonGradingInfo*/
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
        Number, 'studentAvg',

        [ria.serialize.SerializeProperty('itemtypesstats')],
        ArrayOf(chlk.models.grading.ClassPersonGradingItem), 'gradingByAnnouncementTypes'
    ]);
});