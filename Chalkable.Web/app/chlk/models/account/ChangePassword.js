NAMESPACE('chlk.models.account', function () {
    "use strict";
    /** @class chlk.models.account.ChangePassword*/
    CLASS(
        'ChangePassword', [
            String, 'oldPassword',
            String, 'newPassword',
            String, 'newPasswordConfirmation'
        ]);
});
