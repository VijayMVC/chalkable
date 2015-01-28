REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.common.PageWithClasses');
REQUIRE('chlk.models.standard.Standard');
REQUIRE('chlk.models.id.AppId');
REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.models.studyCenter', function(){
    "use strict";

    var SJX = ria.serialize.SJX;

    /**@class chlk.models.studyCenter.PracticeGradeViewData*/

    CLASS(
        UNSAFE, 'PracticeGradeViewData', EXTENDS(chlk.models.common.PageWithClasses), IMPLEMENTS(ria.serialize.IDeserializable), [

            VOID, function deserialize(raw) {
                this.standard = SJX.fromDeserializable(raw.standard, chlk.models.standard.Standard);
                this.practiceScore = SJX.fromValue(raw.practicescore, Number);
                this.gradeBookScore = SJX.fromValue(raw.gradebookscore, Number);
                this.gradedDate = SJX.fromDeserializable(raw.gradeddate, chlk.models.common.ChlkDate);
                this.applicationId = SJX.fromValue(raw.applicationid, chlk.models.id.AppId);
            },

            chlk.models.standard.Standard, 'standard',

            Number, 'practiceScore',

            Number, 'gradeBookScore',

            chlk.models.common.ChlkDate, 'gradedDate',

            chlk.models.id.AppId, 'applicationId'
    ]);
});