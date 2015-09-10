REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.grading.ClassPersonGradingItem');
REQUIRE('chlk.models.grading.StudentAverageInfo');
REQUIRE('chlk.models.classes.Class');

NAMESPACE('chlk.models.grading', function (){
   "use strict";

    var SJX = ria.serialize.SJX;

    /**@class chlk.models.grading.ClassPersonGradingInfo*/
    CLASS(UNSAFE, 'ClassPersonGradingInfo', IMPLEMENTS(ria.serialize.IDeserializable),   [

        chlk.models.id.ClassId, 'classId',

        String, 'className',

        chlk.models.classes.Class, 'clazz',

        chlk.models.grading.ShortStudentAverageInfo, 'studentAvg',

        ArrayOf(chlk.models.grading.ClassPersonGradingItem), 'gradingByAnnouncementTypes',

        chlk.models.grading.StudentAverageInfo, 'studentAverages',

        VOID, function deserialize(raw){
            this.classId = SJX.fromValue(raw.classid, chlk.models.id.ClassId);
            this.clazz = SJX.fromDeserializable(raw.class, chlk.models.classes.Class);
            this.className = SJX.fromValue(raw.classname, String);
            this.studentAvg = SJX.fromDeserializable(raw.gradingperiodavg, chlk.models.grading.ShortStudentAverageInfo);
            this.gradingByAnnouncementTypes = SJX.fromArrayOfDeserializables(raw.items, chlk.models.grading.ClassPersonGradingItem);
            this.studentAverages = SJX.fromArrayOfDeserializables(raw.studentaverages, chlk.models.grading.ShortStudentAverageInfo);
        }
    ]);
});