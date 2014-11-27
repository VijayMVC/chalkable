REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('ria.serialize.SJX');

NAMESPACE('chlk.models.student', function(){
   "use strict";

    var SJX = ria.serialize.SJX;

    /**@class chlk.models.student.StudentGradesHoverBoxItem*/
    CLASS(
        UNSAFE, 'StudentGradesHoverBoxItem', IMPLEMENTS(ria.serialize.IDeserializable), [

        String, 'grade',

        [ria.serialize.SerializeProperty('announcementid')],
        chlk.models.id.AnnouncementId, 'announcementId',

        [ria.serialize.SerializeProperty('announcmenttitle')],
        String, 'announcementTitle',

        String, function getMappedGrade(){
            return this.getGrade(); //todo mapping ...
        },

        VOID, function deserialize(raw){
            this.grade = SJX.fromValue(raw.grade, String);
            this.announcementId = SJX.fromValue(raw.announcementid, chlk.models.id.AnnouncementId);
            this.announcementTitle = SJX.fromValue(raw.announcementtitle, String);
        }
    ]);
});