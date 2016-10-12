REQUIRE('chlk.models.panorama.AdminPanoramaSettingsViewData');
REQUIRE('chlk.models.notification.NotificationsByDate');
REQUIRE('chlk.converters.notification.NotificationTypeToStyleNameConverter');

NAMESPACE('chlk.templates.panorama', function(){
   "use strict";

    /**@class chlk.templates.panorama.AdminPanoramaSettingsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/panorama/AdminPanoramaSettings.jade')],
        [ria.templates.ModelBind(chlk.models.panorama.AdminPanoramaSettingsViewData)],
        'AdminPanoramaSettingsTpl', EXTENDS(chlk.templates.ChlkTemplate),[

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.panorama.CourseTypeSettingViewData), 'courseTypeDefaultSettings',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.profile.StandardizedTestFilterViewData), 'studentDefaultSettings',

            [ria.templates.ModelPropertyBind],
            Number, 'previousYearsCount',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.apps.Application), 'applications'
    ]);
});