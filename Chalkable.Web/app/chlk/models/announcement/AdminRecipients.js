REQUIRE('chlk.models.announcement.AdminRecipientButton');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.AdminRecipients*/
    CLASS(
        'AdminRecipients', [
            ArrayOf(chlk.models.announcement.AdminRecipientButton), 'recipientButtonsInfo',

            Object, 'recipientsData',

            [[ArrayOf(chlk.models.announcement.AdminRecipientButton), Object]],
            function $(recipientButtonsInfo_, recipientsData_){
                BASE();
                if(recipientButtonsInfo_)
                    this.setRecipientButtonsInfo(recipientButtonsInfo_);
                if(recipientsData_)
                    this.setRecipientsData(recipientsData_);
            }
        ]);
});
