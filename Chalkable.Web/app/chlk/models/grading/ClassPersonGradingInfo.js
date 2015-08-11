REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.grading.ClassPersonGradingItem');
REQUIRE('chlk.models.grading.StudentAverageInfo');

NAMESPACE('chlk.models.grading', function (){
   "use strict";

    var SJX = ria.serialize.SJX;

    /**@class chlk.models.grading.ClassPersonGradingInfo*/
    CLASS(UNSAFE, 'ClassPersonGradingInfo', IMPLEMENTS(ria.serialize.IDeserializable),   [

        chlk.models.id.ClassId, 'classId',

        String, 'className',

        Number, 'studentAvg',

        //[ria.serialize.SerializeProperty('itemtypesstats')],
        //ArrayOf(chlk.models.grading.ClassPersonGradingItem), 'gradingByAnnouncementTypes',

        //[ria.serialize.SerializeProperty('studentaverages')],
        //chlk.models.grading.StudentAverageInfo, 'studentAverages',


        VOID, function deserialize(raw){
            this.classId = SJX.fromValue(raw.classid, chlk.models.id.ClassId);
            this.className = SJX.fromValue(raw.classname, String);
            this.studentAvg = SJX.fromValue(raw.calculatedavg, Number);

        }
    ]);
});