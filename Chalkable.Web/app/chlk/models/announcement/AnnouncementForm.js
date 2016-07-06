REQUIRE('chlk.models.classes.ClassesForTopBar');
REQUIRE('chlk.models.classes.ClassForWeekMask');
REQUIRE('chlk.models.announcement.AnnouncementCreate');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.AnnouncementForm*/
    CLASS(
        'AnnouncementForm', EXTENDS(chlk.models.announcement.AnnouncementCreate), [
            chlk.models.classes.ClassesForTopBar, 'topData', //todo: rename
            chlk.models.classes.ClassForWeekMask, 'classInfo',
            Number, 'selectedTypeId',
            String, 'adminRecipientId',
            Array, 'classScheduleDateRanges',
            chlk.models.schoolYear.Year, 'schoolYear',
            Boolean, 'studyCenterEnabled',

            [[chlk.models.classes.ClassesForTopBar, Boolean, chlk.models.schoolYear.Year]],
            function $create(classes, isDraft, schoolYear){
                BASE();
                if(classes){
                    this.setTopData(classes);
                }
                this.setIsDraft(isDraft || false);
                this.schoolYear = schoolYear;
            },

            [[chlk.models.announcement.FeedAnnouncementViewData, chlk.models.schoolYear.Year]],
            function $createFromAnnouncement(announcement, schoolYear){
                BASE();
                this.setAnnouncement(announcement);
                this.schoolYear = schoolYear;
            }
        ]);
});
