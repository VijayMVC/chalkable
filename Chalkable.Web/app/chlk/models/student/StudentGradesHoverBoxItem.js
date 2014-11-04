REQUIRE('chlk.models.id.AnnouncementId');

NAMESPACE('chlk.models.student', function(){
   "use strict";

    /**@class chlk.models.student.StudentGradesHoverBoxItem*/
    CLASS('StudentGradesHoverBoxItem', [

        String, 'grade',

        [ria.serialize.SerializeProperty('announcementid')],
        chlk.models.id.AnnouncementId, 'announcementId',

        [ria.serialize.SerializeProperty('announcmenttitle')],
        String, 'announcementTitle',

        String, function getMappedGrade(){
            return this.getGrade(); //todo mapping ...
        }
    ]);
});