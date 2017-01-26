REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.classes.StudentAlertsViewData');

NAMESPACE('chlk.templates.classes', function () {

    /** @class chlk.templates.classes.StudentAlertsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/class/StudentAlerts.jade')],
        [ria.templates.ModelBind(chlk.models.classes.StudentAlertsViewData)],
        'StudentAlertsTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [[chlk.models.common.AlertInfo]],
            String, function getCssClass(alert){
                var alertTypeEnum = chlk.models.common.AlertTypeEnum;
                switch(alert.getAlertType()){
                    case alertTypeEnum.INTERNET_ACCESS_ALERT: return 'internet';
                    case alertTypeEnum.MEDICAL_ALERT: return 'medical';
                    case alertTypeEnum.SPECIAL_INSTRUCTIONS_ALERT: return 'special';
                    case alertTypeEnum.SPED_STATUS_ALERT: return 'sped';
                    case alertTypeEnum.CUSTOM_STUDENT_ALERT_DETAILS: return 'custom-alert';
                };
                return '';
            }
        ])
});