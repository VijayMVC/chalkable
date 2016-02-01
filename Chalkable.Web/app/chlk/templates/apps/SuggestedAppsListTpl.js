REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.apps.SuggestedAppsList');

NAMESPACE('chlk.templates.apps', function() {
    "use strict";

    /**@class chlk.templates.apps.SuggestedAppsListTpl*/

    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/SuggestedAppsList.jade')],
        [ria.templates.ModelBind(chlk.models.apps.SuggestedAppsList)],
        'SuggestedAppsListTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.standard.Standard), 'standards',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.apps.ApplicationForAttach), 'suggestedApps',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassId, 'classId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AnnouncementId, 'announcementId',

            [ria.templates.ModelPropertyBind],
            chlk.models.announcement.AnnouncementTypeEnum, 'announcementType',

            String, function getStandardsUrlComponents() {
                var standardsWithMoreCodes = (this.standards || []).filter(function(item){
                    var codes = item.getCommonCoreStandardCodesArray();
                    return codes && codes.length > 1;
                });
                var isAllStandardCodes = !standardsWithMoreCodes.length;
                return (this.standards || []).map(function (c, index) { return c.getUrlComponents(index); }).join('&') + '&isAllStandardCodes=' + isAllStandardCodes;
            }
        ]);
});