REQUIRE('chlk.templates.apps.SuggestedAppsListTpl');
REQUIRE('chlk.models.apps.SuggestedAppsList');

NAMESPACE('chlk.templates.apps', function() {
    "use strict";

    /**@class chlk.templates.apps.SuggestedAppsForExplorerListTpl*/

    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/SuggestedAppsListForExplorer.jade')],
        [ria.templates.ModelBind(chlk.models.apps.SuggestedAppsList)],
        'SuggestedAppsForExplorerListTpl', EXTENDS(chlk.templates.apps.SuggestedAppsListTpl), []);
});