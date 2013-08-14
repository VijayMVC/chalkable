REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.common.SimpleResult');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.Account */
    CLASS(
        'AccountService', EXTENDS(chlk.services.BaseService), [
            ria.async.Future, function getRoles() {
                return this.get('chalkable2/app/data/roles.json', ArrayOf(chlk.models.common.NameId), {});
            },

            ria.async.Future, function getRoles() {
                return this.get('chalkable2/app/data/roles.json', ArrayOf(chlk.models.common.NameId), {});
            },

            ria.async.Future, function logOut() {
                return this.post('Home/LogOut.json', chlk.models.common.SimpleResult, {});
            },

            [[String,String,String]],
            ria.async.Future, function changePassword(oldPassword, newPassword, newPasswordConfirmation) {
                return this.get('user/ChangePassword.json', Boolean
                    , {
                        oldPassword: oldPassword,
                        newPassword: newPassword,
                        newPasswordConfirmation: newPasswordConfirmation
                    });
            }

            //doChangePassword(model.oldPassword, model.newPassword, model.newPasswordConfirmation)
        ])
});