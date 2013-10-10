/**
 * Created with JetBrains WebStorm.
 * User: C01t
 * Date: 6/2/13
 * Time: 10:22 AM
 * To change this template use File | Settings | File Templates.
 */

REQUIRE('ria.ajax.JsonPostTask');

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
            }
        ]);
});