REQUIRE('chlk.models.standard.StandardSubject');
REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.models.announcement', function(){
    /**@class chlk.models.announcement.AddStandardViewData*/
    CLASS('AddStandardViewData', [

        String, 'announcementTypeName',

        ArrayOf(chlk.models.standard.StandardSubject), 'itemStandards',

        chlk.models.id.ClassId, 'classId',

        [[String, chlk.models.id.ClassId, ArrayOf(chlk.models.standard.StandardSubject)]],
        function $(announcementTypeName, classId, itemStandards){
            BASE();
            this.setAnnouncementTypeName(announcementTypeName);
            this.setClassId(classId);
            this.setItemStandards(itemStandards);
        }
    ]);
});