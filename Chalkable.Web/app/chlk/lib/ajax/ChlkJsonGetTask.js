REQUIRE('ria.ajax.JsonGetTask');
REQUIRE('chlk.lib.exception.NotAuthorizedException');

NAMESPACE('chlk.lib.ajax', function () {
    "use strict";

    /** @class chlk.lib.ajax.ChlkJsonGetTask */
    CLASS(
        'ChlkJsonGetTask', EXTENDS(ria.ajax.JsonGetTask), [
            function $(url) {
                BASE(url);
            },

            OVERRIDE, VOID, function transferComplete_(evt) {
                if (this._xhr.getResponseHeader('REQUIRES_AUTH') === 1)   {
                    this._completer.completeError(chlk.lib.exception.NotAuthorizedException());
                    return;
                }

                BASE(evt);
            }
        ]);
});