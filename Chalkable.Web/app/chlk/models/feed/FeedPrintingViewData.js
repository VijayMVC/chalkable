REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.announcement.BaseAnnouncementViewData');

NAMESPACE('chlk.models.feed', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.feed.FeedPrintingViewData*/
    CLASS(
        'FeedPrintingViewData', IMPLEMENTS(ria.serialize.IDeserializable), [

            VOID, function deserialize(raw) {
                this.startDate = SJX.fromDeserializable(raw.startdate, chlk.models.common.ChlkDate);
                this.endDate = SJX.fromDeserializable(raw.enddate, chlk.models.common.ChlkDate);
                this.minStart = SJX.fromDeserializable(raw.mindate, chlk.models.common.ChlkDate);
                this.maxEnd = SJX.fromDeserializable(raw.maxdate, chlk.models.common.ChlkDate);
                this.lessonPlanOnly = SJX.fromValue(raw.lessonplanonly, Boolean);
                this.includeDetails = SJX.fromValue(raw.includedetails, Boolean);
                this.includeAttachments = SJX.fromValue(raw.includeattachments, Boolean);
                this.includeHiddenActivities = SJX.fromValue(raw.includehiddenactivities, Boolean);
                this.includeHiddenAttributes = SJX.fromValue(raw.includehiddenattributes, Boolean);
                this.groupByStandards = SJX.fromValue(raw.groupedbystandards, Boolean);
                this.editableLPOption = SJX.fromValue(raw.editablelpoption, Boolean);
                this.importantOnly = SJX.fromValue(raw.importantonly, Boolean);
                this.announcementType = raw.announcementtype ? SJX.fromValue(raw.announcementtype, chlk.models.announcement.AnnouncementTypeEnum) : null;
                this.classId = SJX.fromValue(raw.classid, chlk.models.id.ClassId);
            },

            chlk.models.common.ChlkDate, 'startDate',

            chlk.models.common.ChlkDate, 'endDate',

            chlk.models.common.ChlkDate, 'minStart',

            chlk.models.common.ChlkDate, 'maxEnd',

            Boolean, 'lessonPlanOnly',

            Boolean, 'includeDetails',

            Boolean, 'includeAttachments',

            Boolean, 'includeHiddenActivities',

            Boolean, 'includeHiddenAttributes',

            Boolean, 'groupByStandards',

            Boolean, 'editableLPOption',

            Boolean, 'importantOnly',

            chlk.models.announcement.AnnouncementTypeEnum, 'announcementType',

            chlk.models.id.ClassId, 'classId'
        ]);
});