REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.common.PermissionsErrorTpl');

NAMESPACE('chlk.activities.common', function(){
   "use strict";
    /**@class chlk.activities.common.PermissionsErrorPage*/

    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.common.PermissionsErrorTpl)],
        'PermissionsErrorPage', EXTENDS(chlk.activities.lib.TemplatePage),[]);
});