REQUIRE('chlk.models.apps.AppPicture');

NAMESPACE('chlk.templates.apps', function () {

    /** @class chlk.templates.apps.AppPicture*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/app-picture.jade')],
        [ria.templates.ModelBind(chlk.models.apps.AppPicture)],
        'AppPicture', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.id.PictureId, 'pictureId',
            [ria.templates.ModelPropertyBind],
            String, 'pictureUrl',
            [ria.templates.ModelPropertyBind],
            Number, 'width',
            [ria.templates.ModelPropertyBind],
            Number, 'height',
            [ria.templates.ModelPropertyBind],
            String, 'title',
            [ria.templates.ModelPropertyBind],
            Boolean, 'editable',
            [ria.templates.ModelPropertyBind],
            String, 'pictureClass',
            [ria.templates.ModelPropertyBind],
            String, 'templateDownloadLink'

        ])
});