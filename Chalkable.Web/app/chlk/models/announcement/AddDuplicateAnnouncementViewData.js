REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.classes.ClassesForTopBar');
REQUIRE('chlk.models.classes.ClassForTopBar');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.AddDuplicateAnnouncementViewData*/
    CLASS(
        'AddDuplicateAnnouncementViewData',[

            ArrayOf(chlk.models.classes.ClassForTopBar), 'classes', //todo: rename
            chlk.models.id.ClassId, 'selectedClassId',
            chlk.models.id.AnnouncementId, 'announcementId',
            String, 'selectedIds',

            [[chlk.models.id.AnnouncementId, ArrayOf(chlk.models.classes.ClassForTopBar), chlk.models.id.ClassId]],
            function $(announcementId_, classes_, selectedClassId_){
                BASE();
                if(announcementId_)
                    this.setAnnouncementId(announcementId_);
                if(classes_)
                    this.setClasses(classes_);
                if(selectedClassId_){
                    this.setSelectedClassId(selectedClassId_);
                    this.setSelectedIds(selectedClassId_.valueOf().toString())
                }
            }
         ]);
});
