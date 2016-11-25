REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.profile.PersonProfileSummaryTpl');

NAMESPACE('chlk.activities.profile', function(){
    "use strict";

    /**@class chlk.activities.profile.PersonProfileSummaryPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.profile.PersonProfileSummaryTpl)],
        'PersonProfileSummaryPage', EXTENDS(chlk.activities.lib.TemplatePage),[

    ]);
});