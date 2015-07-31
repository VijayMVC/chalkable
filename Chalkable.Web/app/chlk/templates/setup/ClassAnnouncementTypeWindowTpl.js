REQUIRE('chlk.models.announcement.ClassAnnouncementType');
REQUIRE('chlk.templates.ChlkTemplate');

NAMESPACE('chlk.templates.setup', function () {

    /** @class chlk.templates.setup.ClassAnnouncementTypeWindowTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/setup/ClassAnnouncementTypeWindow.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.ClassAnnouncementType)],
        'ClassAnnouncementTypeWindowTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            Number, 'id',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassId, 'classId',

            [ria.templates.ModelPropertyBind],
            Number, 'highScoresToDrop',

            [ria.templates.ModelPropertyBind],
            Number, 'lowScoresToDrop',

            [ria.templates.ModelPropertyBind],
            Number, 'percentage',

            [ria.templates.ModelPropertyBind],
            String, 'name',

            [ria.templates.ModelPropertyBind],
            String, 'description',

            [ria.templates.ModelPropertyBind],
            Boolean, 'system',

            [ria.templates.ModelPropertyBind],
            Boolean, 'ableEdit'
        ])
});
