REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.common.BaseAttachViewData');

NAMESPACE('chlk.templates.common', function () {

    /** @class chlk.templates.common.BaseAttachTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/attach-apps-dialog.jade')],
        [ria.templates.ModelBind(chlk.models.common.BaseAttachViewData)],
        'BaseAttachTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AnnouncementId, 'announcementId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassId, 'classId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AppId, 'assessmentAppId',

            [ria.templates.ModelPropertyBind],
            Boolean, 'fileCabinetEnabled',

            [ria.templates.ModelPropertyBind],
            Boolean, 'standardAttachEnabled',

            [ria.templates.ModelPropertyBind],
            Boolean, 'showApps',

            [ria.templates.ModelPropertyBind],
            String, 'announcementTypeName',

            [ria.templates.ModelPropertyBind],
            chlk.models.announcement.AnnouncementTypeEnum, 'announcementType'
        ])
});