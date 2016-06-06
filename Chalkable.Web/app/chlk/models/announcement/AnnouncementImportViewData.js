REQUIRE('chlk.models.announcement.FeedAnnouncementViewData');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.AnnouncementImportViewData*/
    CLASS('AnnouncementImportViewData', [
        ArrayOf(chlk.models.announcement.FeedAnnouncementViewData), 'announcements',

        ArrayOf(chlk.models.schoolYear.YearAndClasses), 'classesByYears',

        chlk.models.id.ClassId, 'classId',

        String, 'requestId',

        Array, 'classScheduleDateRanges',

        [[chlk.models.id.ClassId, ArrayOf(chlk.models.schoolYear.YearAndClasses), ArrayOf(chlk.models.announcement.FeedAnnouncementViewData), String, Array]],
        function $(classId_, classesByYears_, announcements_, requestId_, classScheduleDateRanges_){
            BASE();
            classId_ && this.setClassId(classId_);
            requestId_ && this.setRequestId(requestId_);
            classesByYears_ && this.setClassesByYears(classesByYears_);
            announcements_ && this.setAnnouncements(announcements_);
            classScheduleDateRanges_ && this.setClassScheduleDateRanges(classScheduleDateRanges_);
        }
    ]);
});
