REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.common.AlertInfo');

NAMESPACE('chlk.templates.common', function () {

    /** @class chlk.templates.common.AlertsPopUpTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/common/AlertsPopUpTpl.jade')],
        [ria.templates.ModelBind(chlk.models.common.Alerts)],
        'AlertsPopUpTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.common.AlertInfo), 'alerts',

            [ria.templates.ModelPropertyBind],
            String, 'stringAlerts',

            [[chlk.models.common.AlertInfo]],
            String, function getCssClass(alert){
                var alertTypeEnum = chlk.models.common.AlertTypeEnum;
                switch(alert.getAlertType()){
                    case alertTypeEnum.INTERNET_ACCESS_ALERT: return 'internet';
                    case alertTypeEnum.MEDICAL_ALERT: return 'medical';
                    case alertTypeEnum.SPECIAL_INSTRUCTIONS_ALERT: return 'special';
                    case alertTypeEnum.SPED_STATUS_ALERT: return 'sped';
                };
                return '';
            }
        ]);
});