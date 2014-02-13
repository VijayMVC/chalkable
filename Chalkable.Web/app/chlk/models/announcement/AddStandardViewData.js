REQUIRE('chlk.models.standard.StandardSubject');
REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.models.announcement', function(){
    /**@class chlk.models.announcement.AddStandardViewData*/
    CLASS('AddStandardViewData', [

        String, 'announcementTypeName',

        ArrayOf(chlk.models.standard.StandardSubject), 'itemStandards',

        chlk.models.id.AnnouncementId, 'announcementId',

        chlk.models.id.ClassId, 'classId',

        Array, 'standardIds',

        [[String, chlk.models.id.AnnouncementId, chlk.models.id.ClassId, ArrayOf(chlk.models.standard.StandardSubject), Array]],
        function $(announcementTypeName, announcementId, classId, itemStandards, standardIds){
            BASE();
            this.setAnnouncementId(announcementId);
            this.setAnnouncementTypeName(announcementTypeName);
            this.setClassId(classId);
            this.setItemStandards(itemStandards);
            this.setStandardIds(standardIds);
        }
    ]);
});