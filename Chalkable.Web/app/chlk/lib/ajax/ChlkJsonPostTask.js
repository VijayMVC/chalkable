REQUIRE('ria.ajax.JsonPostTask');
REQUIRE('chlk.lib.exception.NotAuthorizedException');

NAMESPACE('chlk.lib.ajax', function () {
    "use strict";

    /** @class chlk.lib.ajax.ChlkJsonPostTask */
    CLASS(
        'ChlkJsonPostTask', EXTENDS(ria.ajax.JsonPostTask), [
            function $(url) {
                BASE(url);
            },

            OVERRIDE, Object, function getBody_() {
                return this._method != ria.ajax.Method.GET ? JSON.stringify(this._params) : '';
            },

            OVERRIDE, VOID, function transferComplete_(evt) {
                if (this._xhr.status == 401)   {
                    this._completer.completeError(chlk.lib.exception.NotAuthorizedException());
                    return;
                }
                BASE(evt);
            }
        ]);
});