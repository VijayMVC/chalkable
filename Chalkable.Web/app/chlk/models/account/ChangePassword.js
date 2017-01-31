NAMESPACE('chlk.models.account', function () {
    "use strict";
    /** @class chlk.models.account.ChangePassword*/
    CLASS(
        'ChangePassword', [
            String, 'oldPassword',
            String, 'newPassword',
            String, 'newPasswordConfirmation',
            Boolean, "withOldPassword",

            function $(oldPassword_, newPassword_, newPasswordConfirmation_, withOldPassword_){
                BASE();
                oldPassword_ && this.setOldPassword(oldPassword_);
                newPassword_ && this.setNewPassword(newPassword_);
                newPasswordConfirmation_ && this.setNewPasswordConfirmation(newPasswordConfirmation_);
                withOldPassword_ && this.setWithOldPassword(withOldPassword_);
            }
        ]);
});
