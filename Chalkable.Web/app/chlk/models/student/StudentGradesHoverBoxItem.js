
NAMESPACE('chlk.models.student', function(){
   "use strict";

    /**@class chlk.models.student.StudentGradesHoverBoxItem*/
    CLASS('StudentGradesHoverBoxItem', [

        String, 'grade',
        [ria.serialize.SerializeProperty('gradingstyle')],
        Number, 'gradingStyle',
        [ria.serialize.SerializeProperty('announcementtypeid')],
        Number, 'announcementTypeId',
        [ria.serialize.SerializeProperty('announcmenttitle')],
        String, 'announcementTitle',

        String, function getMappedGrade(){
            return this.getGrade(); //todo mapping ...
        }
    ]);
});