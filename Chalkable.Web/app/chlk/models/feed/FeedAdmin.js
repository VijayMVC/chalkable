REQUIRE('chlk.models.common.PageWithGrades');
REQUIRE('chlk.models.announcement.FeedAnnouncementViewData');
REQUIRE('chlk.models.schoolYear.GradingPeriod');

NAMESPACE('chlk.models.feed', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.feed.FeedAdmin*/
    CLASS(
        'FeedAdmin',
                EXTENDS(chlk.models.common.PageWithGrades),
                IMPLEMENTS(ria.serialize.IDeserializable), [

            VOID, function deserialize(raw) {
                this.inProfile = SJX.fromValue(raw.inProfile, Boolean);
                this.items = SJX.fromArrayOfDeserializables(raw.annoucementviewdatas, chlk.models.announcement.FeedAnnouncementViewData);
                this.importantOnly = SJX.fromValue(raw.importantOnly, Boolean);
                this.toSet = SJX.fromValue(raw.toSet, Boolean);
                this.importantCount = SJX.fromValue(raw.importantCount, Number);
                this.newNotificationCount = SJX.fromValue(raw.newNotificationCount, Number);
                this.start = SJX.fromValue(raw.start, Number);
                this.count = SJX.fromValue(raw.count, Number);
                this.startDate = SJX.fromDeserializable(raw.settingsforfeed ? raw.settingsforfeed.fromdate : raw.fromdate, chlk.models.common.ChlkDate);
                this.endDate = SJX.fromDeserializable(raw.settingsforfeed ? raw.settingsforfeed.todate : raw.todate, chlk.models.common.ChlkDate);
                this.annType = SJX.fromValue(raw.settingsforfeed ? raw.settingsforfeed.announcementtype : raw.announcementtype, chlk.models.announcement.AnnouncementTypeEnum);
                this.gradingPeriodId = SJX.fromValue(raw.settingsforfeed ? raw.settingsforfeed.gradingperiodid : raw.gradingperiodid, chlk.models.id.GradingPeriodId);
                this.sortType = SJX.fromValue(raw.settingsforfeed ? raw.settingsforfeed.sorttype : raw.sorttype, chlk.models.announcement.FeedSortTypeEnum);
                this.submitType = SJX.fromValue(raw.submitType, String);
                this.markDoneOption = SJX.fromValue(raw.markDoneOption, Number);
                this.gradeLevels = SJX.fromValue(raw.gradeLevels, String);
            },

            [[ArrayOf(chlk.models.announcement.FeedAnnouncementViewData), chlk.models.grading.GradeLevelsForTopBar, Boolean, Number]],
            function $(items_, gradeLevels_, importantOnly_, newNotificationCount_){
                BASE(gradeLevels_);
                if(items_)
                    this.setItems(items_);
                if(importantOnly_ !== undefined)
                    this.setImportantOnly(importantOnly_);
                if(newNotificationCount_)
                    this.setNewNotificationCount(newNotificationCount_);
            },

            ArrayOf(chlk.models.announcement.FeedAnnouncementViewData), 'items',

            Boolean, 'importantOnly',

            Number, 'importantCount',

            Number, 'start',

            Number, 'count',

            Number, 'markDoneOption',

            Number, 'newNotificationCount',

            chlk.models.common.ChlkDate, 'startDate',

            chlk.models.common.ChlkDate, 'endDate',

            chlk.models.announcement.AnnouncementTypeEnum, 'annType',

            ArrayOf(chlk.models.schoolYear.GradingPeriod), 'gradingPeriods',

            chlk.models.id.GradingPeriodId, 'gradingPeriodId',

            chlk.models.announcement.FeedSortTypeEnum, 'sortType',

            String, 'submitType',

            String, 'gradeLevels',

            Boolean, 'inProfile',

            Boolean, 'toSet',

            Boolean, 'staringDisabled',

            Boolean, 'readonly'
        ]);
});