REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.grading.ClassPersonGradingItem');
REQUIRE('chlk.models.grading.StudentAverageInfo');

NAMESPACE('chlk.models.grading', function (){
   "use strict";

    /**@class chlk.models.grading.ClassPersonGradingInfo*/
    CLASS('ClassPersonGradingInfo',  [

        [ria.serialize.SerializeProperty('classid')],
        chlk.models.id.ClassId, 'classId',

        [ria.serialize.SerializeProperty('classname')],
        String, 'className',

        [ria.serialize.SerializeProperty('studentavg')],
        Number, 'studentAvg',

        [ria.serialize.SerializeProperty('itemtypesstats')],
        ArrayOf(chlk.models.grading.ClassPersonGradingItem), 'gradingByAnnouncementTypes',

        [ria.serialize.SerializeProperty('studentaverages')],
        chlk.models.grading.StudentAverageInfo, 'studentAverages'
    ]);
});