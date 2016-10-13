REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.id.GradingPeriodId');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.AnnouncementTypeGradingId');
REQUIRE('chlk.models.id.StandardId');


NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.SubmitDroppedAnnouncementViewData*/
    CLASS(
        'SubmitDroppedAnnouncementViewData', [
            chlk.models.id.AnnouncementId, 'announcementId',
            chlk.models.id.GradingPeriodId, 'gradingPeriodId',
            chlk.models.id.ClassId, 'classId',
            chlk.models.id.AnnouncementTypeGradingId, 'categoryId',
            chlk.models.id.StandardId, 'standardId',
            Boolean, 'dropped',

            [[
                chlk.models.id.AnnouncementId,
                Boolean,
                chlk.models.id.GradingPeriodId,
                chlk.models.id.ClassId,
                chlk.models.id.AnnouncementTypeGradingId,
                chlk.models.id.StandardId
            ]],
            function $(announcementId_, dropped_, gradingPeriodId_, classId_, categoryId_, standardId_){
                BASE();
                if(announcementId_)
                    this.setAnnouncementId(announcementId_);
                if(dropped_)
                    this.setDropped(dropped_);
                if(gradingPeriodId_)
                    this.setGradingPeriodId(gradingPeriodId_);
                if(classId_)
                    this.setClassId(classId_);
                if(categoryId_)
                    this.setCategoryId(categoryId_);
                if(standardId_)
                    this.setStandardId(standardId_);
            }
        ]);
});
