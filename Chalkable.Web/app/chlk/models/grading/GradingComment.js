NAMESPACE('chlk.models.grading', function () {
    "use strict";
    /** @class chlk.models.grading.GradingComment*/
    CLASS(
        'GradingComment', [
            String, 'comment',

            [[String]],
            function $(comment){
                BASE();
                this.setComment(comment);
            }
        ]);
});
