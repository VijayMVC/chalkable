REQUIRE('ria.serialize.SJX');
REQUIRE('ria.serialize.IDeserializable');
REQUIRE('chlk.models.id.StandardId');
REQUIRE('chlk.models.id.AnnouncementId');

NAMESPACE('chlk.models.standard', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.standard.Standard*/
    CLASS(UNSAFE, FINAL, 'Standard', IMPLEMENTS(ria.serialize.IDeserializable),  [
        VOID, function deserialize(raw) {
            this.name = SJX.fromValue(raw.name, String);
            this.description = SJX.fromValue(raw.description, String);
            this.announcementId = SJX.fromValue(raw.announcementid, chlk.models.id.AnnouncementId);
            this.standardId = SJX.fromValue(raw.standardid, chlk.models.id.StandardId);
            this.grade = SJX.fromValue(raw.grade, String);
            this.commonCoreStandardCode = SJX.fromValue(raw.ccstandardcode, String);
        },

        String, 'name',
        String, 'description',
        chlk.models.id.AnnouncementId, 'announcementId',
        chlk.models.id.StandardId, 'standardId',
        String, 'grade',

        String, 'commonCoreStandardCode',

        [[chlk.models.id.StandardId, String, String, String]],
        function $(standardId_, name_, grade_, ccStandardCode_){
            BASE();
            if(standardId_)
                this.setStandardId(standardId_);
            if(name_)
                this.setName(name_);
            if(grade_)
                this.setGrade(grade_);
            if(ccStandardCode_)
                this.setCommonCoreStandardCode(ccStandardCode_);
        }
        ]);
});
