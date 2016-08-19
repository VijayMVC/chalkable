REQUIRE('chlk.models.id.HealthFormId');
REQUIRE('chlk.models.id.SchoolPersonId');

NAMESPACE('chlk.models.student', function () {
    "use strict";

    /** @class chlk.models.student.VerifyHealthFormViewData*/
    CLASS(
        'VerifyHealthFormViewData',  [

            String, 'requestId',
            chlk.models.id.HealthFormId, 'healthFormId',
            chlk.models.id.SchoolPersonId, 'studentId',
            String, 'documentUrl',
            Boolean, 'readonly',

            [[String, chlk.models.id.SchoolPersonId, chlk.models.id.HealthFormId, String, Boolean]],
            function $(requestId_,  studentId_, healthFormId_, documentUrl_, readonly_){
                BASE();
                if(requestId_)
                    this.setRequestId(requestId_);
                if(readonly_)
                    this.setReadonly(readonly_);
                if(healthFormId_)
                    this.setHealthFormId(healthFormId_);
                if(studentId_)
                    this.setStudentId(studentId_);
                if(documentUrl_)
                    this.setDocumentUrl(documentUrl_);
            }

        ]);
});
