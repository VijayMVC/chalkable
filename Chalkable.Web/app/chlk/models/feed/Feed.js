REQUIRE('chlk.models.common.PageWithClasses');
REQUIRE('chlk.models.announcement.FeedAnnouncementViewData');
REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.models.feed', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.feed.Feed*/
    CLASS(
        'Feed',
                EXTENDS(chlk.models.common.PageWithClasses),
                IMPLEMENTS(ria.serialize.IDeserializable), [

            VOID, function deserialize(raw) {
                this.items = SJX.fromArrayOfDeserializables(raw.annoucementviewdatas, chlk.models.announcement.FeedAnnouncementViewData);
                this.importantOnly = SJX.fromValue(raw.importantOnly, Boolean);
                this.importantCount = SJX.fromValue(raw.importantCount, Number);
                this.newNotificationCount = SJX.fromValue(raw.newNotificationCount, Number);
                this.start = SJX.fromValue(raw.start, Number);
                this.count = SJX.fromValue(raw.count, Number);
                this.startDate = SJX.fromDeserializable(raw.fromdate, chlk.models.common.ChlkDate);
                this.endDate = SJX.fromDeserializable(raw.todate, chlk.models.common.ChlkDate);
                this.lessonPlansOnly = SJX.fromValue(raw.lessonplansonly, Boolean);
                this.submitType = SJX.fromValue(raw.submitType, String);
                this.markDoneOption = SJX.fromValue(raw.markDoneOption, Number);
                this.latest = SJX.fromValue(raw.sorttype, Boolean);
                this.classId = SJX.fromValue(raw.classId, chlk.models.id.ClassId);
            },

            [[ArrayOf(chlk.models.announcement.FeedAnnouncementViewData), chlk.models.classes.ClassesForTopBar, Boolean, Number]],
            function $(items_, classes_, importantOnly_, newNotificationCount_, startDate_, endDate_){
                BASE(classes_);
                if(items_)
                    this.setItems(items_);
                if(importantOnly_)
                    this.setImportantOnly(importantOnly_);
                if(newNotificationCount_)
                    this.setNewNotificationCount(newNotificationCount_);
                if(startDate_)
                    this.setStartDate(startDate_);
                if(endDate_)
                    this.setEndDate(endDate_);
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

            Boolean, 'lessonPlansOnly',

            Boolean, 'latest',

            String, 'submitType',

            chlk.models.id.ClassId, 'classId'
        ]);
});