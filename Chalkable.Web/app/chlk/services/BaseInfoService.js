REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('ria.async.Observable');


NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.UserInfoChangeEvent */
    DELEGATE(
        [[String]],
        VOID, function UserInfoChangeEvent(string) {});

    /** @class chlk.services.BaseInfoService */
    CLASS(
        'BaseInfoService', EXTENDS(chlk.services.BaseService), [

            READONLY, ria.async.IObservable, 'userInfoChange',

            function $() {
                BASE();
                this.userInfoChange = new ria.async.Observable(chlk.services.UserInfoChangeEvent);
            }
        ])
});