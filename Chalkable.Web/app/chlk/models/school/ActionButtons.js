REQUIRE('chlk.models.Popup');

NAMESPACE('chlk.models.school', function () {
    "use strict";
    /** @class chlk.models.school.ActionButtons*/
    CLASS(
        'ActionButtons', EXTENDS(chlk.models.Popup), [
            ArrayOf(Number), 'buttons',
            ArrayOf(String), 'emails',

            [[ArrayOf(Number), ArrayOf(String), ria.dom.Dom, ria.dom.Dom]],
            function $(buttrons, emails, target, container_){
                BASE(target, container_);
                this.setButtons(buttrons);
                this.setEmails(emails);
            }
        ]);
});
