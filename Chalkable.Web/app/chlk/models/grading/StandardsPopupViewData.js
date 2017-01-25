REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.id.StandardId');

NAMESPACE('chlk.models.grading', function () {
    "use strict";

    /** @class chlk.models.grading.StandardsPopupViewData*/
    CLASS(
        'StandardsPopupViewData', [
            chlk.models.id.SchoolPersonId, 'studentId',
            chlk.models.id.StandardId, 'standardId',
            Array, 'items',

            function $(studentId_, standardId_, items_){
                BASE();
                studentId_ && this.setStudentId(studentId_);
                standardId_ && this.setStandardId(standardId_);
                items_ && this.setItems(items_);
            }
        ]);
});
