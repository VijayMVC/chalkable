REQUIRE('chlk.templates.common.BaseAttachTpl');
REQUIRE('chlk.models.apps.ExternalAttachAppViewData');


NAMESPACE('chlk.templates.apps', function () {

    /** @class chlk.templates.apps.ExternalAttachAppDialogTpl */
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/external-attach-app-dialog.jade')],
        [ria.templates.ModelBind(chlk.models.apps.ExternalAttachAppViewData)],
        'ExternalAttachAppDialogTpl', EXTENDS(chlk.templates.common.BaseAttachTpl), [

            [ria.templates.ModelPropertyBind],
            chlk.models.apps.Application, 'app',
            [ria.templates.ModelPropertyBind],
            String, 'url',
            [ria.templates.ModelPropertyBind],
            String, 'title',
            [ria.templates.ModelPropertyBind],
            chlk.models.id.AnnouncementApplicationId, 'announcementApplicationId',

            function isAttachButtonVisible() {
                var isAssessmentApp = this.getApp().getId().valueOf() == this.getAttachOptions().getAssessmentAppId().valueOf();
                var isNewAssessmentEnabled = this.getSession().get(ChlkSessionConstants.ASSESSMENT_ENABLED, false);

                return isAssessmentApp && !isNewAssessmentEnabled;
            }
        ]);
});
