REQUIRE('chlk.models.common.PageWithGrades');
REQUIRE('chlk.models.announcement.FeedAnnouncementViewData');

NAMESPACE('chlk.models.feed', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.feed.FeedAdmin*/
    CLASS(
        'FeedAdmin',
                EXTENDS(chlk.models.common.PageWithGrades),
                IMPLEMENTS(ria.serialize.IDeserializable), [

            VOID, function deserialize(raw) {
                this.items = SJX.fromArrayOfDeserializables(raw.items, chlk.models.announcement.FeedAnnouncementViewData);
                this.importantOnly = SJX.fromValue(raw.importantOnly, Boolean);
                this.importantCount = SJX.fromValue(raw.importantCount, Number);
                this.newNotificationCount = SJX.fromValue(raw.newNotificationCount, Number);
                this.start = SJX.fromValue(raw.start, Number);
                this.count = SJX.fromValue(raw.count, Number);
                this.startDate = SJX.fromDeserializable(raw.startdate, chlk.models.common.ChlkDate);
                this.endDate = SJX.fromDeserializable(raw.enddate, chlk.models.common.ChlkDate);
                this.lessonPlansOnly = SJX.fromValue(raw.lessonplansonly, Boolean);
                this.latest = SJX.fromValue(raw.latest, Boolean);
                this.submitType = SJX.fromValue(raw.submitType, String);
                this.markDoneOption = SJX.fromValue(raw.markDoneOption, Number);
                this.gradeLevels = SJX.fromValue(raw.gradeLevels, String);
            },

            [[ArrayOf(chlk.models.announcement.FeedAnnouncementViewData), chlk.models.grading.GradeLevelsForTopBar, Boolean, Number]],
            function $(items_, gradeLevels_, importantOnly_, newNotificationCount_){
                BASE(gradeLevels_);
                if(items_)
                    this.setItems(items_);
                if(importantOnly_)
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

            Boolean, 'lessonPlansOnly',

            Boolean, 'latest',

            String, 'submitType',

            String, 'gradeLevels'
        ]);
});