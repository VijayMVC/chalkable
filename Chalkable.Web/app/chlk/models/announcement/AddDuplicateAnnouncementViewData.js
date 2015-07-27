REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.classes.ClassesForTopBar');
REQUIRE('chlk.models.classes.Class');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.AddDuplicateAnnouncementViewData*/
    CLASS(
        'AddDuplicateAnnouncementViewData',[

            ArrayOf(chlk.models.classes.Class), 'classes', //todo: rename
            chlk.models.id.ClassId, 'selectedClassId',
            chlk.models.id.AnnouncementId, 'announcementId',
            String, 'selectedIds',
            chlk.models.announcement.AnnouncementTypeEnum, 'type',

            [[chlk.models.id.AnnouncementId, ArrayOf(chlk.models.classes.Class), chlk.models.id.ClassId, chlk.models.announcement.AnnouncementTypeEnum]],
            function $(announcementId_, classes_, selectedClassId_, type_){
                BASE();
                if(announcementId_)
                    this.setAnnouncementId(announcementId_);
                if(classes_)
                    this.setClasses(classes_);
                if(selectedClassId_){
                    this.setSelectedClassId(selectedClassId_);
                    this.setSelectedIds(selectedClassId_.valueOf().toString())
                }
                if(type_)
                    this.setType(type_);
            }
         ]);
});
