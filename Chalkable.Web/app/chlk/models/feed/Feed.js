REQUIRE('chlk.models.common.PageWithClasses');
REQUIRE('chlk.models.announcement.FeedAnnouncementViewData');
REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.schoolYear.GradingPeriod');
REQUIRE('chlk.models.schoolYear.YearAndClasses');

NAMESPACE('chlk.models.feed', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.feed.Feed*/
    CLASS(
        'Feed',
                EXTENDS(chlk.models.common.PageWithClasses),
                IMPLEMENTS(ria.serialize.IDeserializable), [

            VOID, function deserialize(raw) {

                //Prepare created announcements
                if(raw.createdannouncements && raw.createdannouncements.length){
                    raw.createdannouncements.forEach(function(item){
                        item.imported = true;
                    });

                    raw.annoucementviewdatas = raw.createdannouncements.concat(raw.annoucementviewdatas);
                }

                this.selectedAnnouncements = SJX.fromValue(raw.selectedAnnouncements, String);
                this.toClassId = SJX.fromValue(raw.toClassId, chlk.models.id.ClassId);
                this.copyStartDate = SJX.fromDeserializable(raw.copyStartDate, chlk.models.common.ChlkDate);
                this.adjustStartDate = SJX.fromDeserializable(raw.adjustStartDate, chlk.models.common.ChlkDate);
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
                this.submitType = SJX.fromValue(raw.submitType, String);
                this.markDoneOption = SJX.fromValue(raw.markDoneOption, Number);
                this.sortType = SJX.fromValue(raw.settingsforfeed ? raw.settingsforfeed.sorttype : raw.sorttype, chlk.models.announcement.FeedSortTypeEnum);
                this.classId = SJX.fromValue(raw.classId, chlk.models.id.ClassId);
                //this.createdAnnouncements = SJX.fromArrayOfDeserializables(raw.createdannouncements, chlk.models.announcement.FeedAnnouncementViewData);
            },

            [[ArrayOf(chlk.models.announcement.FeedAnnouncementViewData), chlk.models.classes.ClassesForTopBar, Boolean, Number]],
            function $(items_, classes_, importantOnly_, newNotificationCount_, startDate_, endDate_){
                BASE(classes_);
                if(items_)
                    this.setItems(items_);
                if(importantOnly_ !== undefined)
                    this.setImportantOnly(importantOnly_);
                if(newNotificationCount_)
                    this.setNewNotificationCount(newNotificationCount_);
                if(startDate_)
                    this.setStartDate(startDate_);
                if(endDate_)
                    this.setEndDate(endDate_);
            },

            ArrayOf(chlk.models.announcement.FeedAnnouncementViewData), 'items',

            ArrayOf(chlk.models.announcement.FeedAnnouncementViewData), 'createdAnnouncements',

            Boolean, 'readonly',

            String, 'selectedAnnouncements',

            chlk.models.id.ClassId, 'toClassId',

            chlk.models.common.ChlkDate, 'copyStartDate',

            chlk.models.common.ChlkDate, 'adjustStartDate',

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

            ArrayOf(chlk.models.schoolYear.YearAndClasses), 'classesByYears',

            chlk.models.id.GradingPeriodId, 'gradingPeriodId',

            chlk.models.announcement.FeedSortTypeEnum, 'sortType',

            String, 'submitType',

            chlk.models.id.ClassId, 'classId',

            Boolean, 'toSet',

            Boolean, 'staringDisabled',

            Boolean, 'inProfile'
        ]);
});