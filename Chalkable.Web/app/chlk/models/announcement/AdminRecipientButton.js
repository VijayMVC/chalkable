REQUIRE('chlk.models.id.ReminderId');
REQUIRE('chlk.models.id.AnnouncementId');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.AdminRecipientButton*/
    CLASS(
        'AdminRecipientButton', [
            String, 'id',

            Number, 'roleId',

            String, 'text',

            Number, 'index',

            [[String, Number, String, Number]],
            function $(id_, roleId_, text_, index_){
                BASE();
                if(id_)
                    this.setId(id_);
                if(roleId_)
                    this.setRoleId(roleId_);
                if(text_)
                    this.setText(text_);
                if(index_)
                    this.setIndex(index_);
            }
        ]);
});
