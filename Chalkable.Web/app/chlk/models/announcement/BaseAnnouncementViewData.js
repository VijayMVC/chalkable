REQUIRE('ria.serialize.SJX');
REQUIRE('ria.serialize.IDeserializable');
REQUIRE('chlk.models.id.AnnouncementId');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.announcement.BaseAnnouncementViewData*/
    CLASS(
        UNSAFE, 'BaseAnnouncementViewData', IMPLEMENTS(ria.serialize.IDeserializable), [

            VOID, function deserialize(raw) {
                this.announcementTypeName = SJX.fromValue(raw.announcementtypename, String);
                this.ableDropStudentScore = SJX.fromValue(raw.candropstudentscore, Boolean);
                this.ableExemptStudentScore = SJX.fromValue(raw.maybeexempt, Boolean);
                this.order = SJX.fromValue(raw.order, Number);
                this.title = SJX.fromValue(raw.title, String);
                this.expiresDate = SJX.fromDeserializable(raw.expiresdate, chlk.models.common.ChlkDate);
                this.avg = SJX.fromValue(raw.avg, Number);
                this.id = SJX.fromValue(Number(raw.id), chlk.models.id.AnnouncementId);
                this.dropped = SJX.fromValue(raw.dropped, Boolean);
                this.maxScore = SJX.fromValue(raw.maxscore, Number);
                this.annOwner = SJX.fromValue(raw.isowner, Boolean);
                this.gradable = SJX.fromValue(raw.gradable, Boolean);
                this.ableToGrade = SJX.fromValue(raw.cangrade, Boolean);
            },

            String, 'announcementTypeName',
            Boolean, 'ableDropStudentScore',
            Boolean, 'ableExemptStudentScore',
            Number, 'order',
            String, 'title',
            chlk.models.common.ChlkDate, 'expiresDate',
            Number, 'avg',
            chlk.models.id.AnnouncementId, 'id',
            Boolean, 'dropped',
            Number, 'maxScore',
            Boolean, 'annOwner',
            Boolean, 'gradable',
            Boolean, 'ableToGrade'
        ]);
});
