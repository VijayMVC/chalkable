REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.profile.SchoolPersonProfileAppsTpl');

NAMESPACE('chlk.activities.profile', function(){
   "use strict";
    /**@class chlk.activities.profile.SchoolPersonAppsPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.profile.SchoolPersonProfileAppsTpl)],
        'SchoolPersonAppsPage', EXTENDS(chlk.activities.lib.TemplatePage),[
    ]);
});