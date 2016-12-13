REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.classes.Class');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.CourseId');
REQUIRE('chlk.models.common.InfoMsg');

NAMESPACE('chlk.templates.common', function () {

    /** @class chlk.templates.common.InfoMsg*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/common/InfoMsg.jade')],
        [ria.templates.ModelBind(chlk.models.common.InfoMsg)],
        'InfoMsg', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            String, 'text',
            [ria.templates.ModelPropertyBind],
            String, 'header',
            [ria.templates.ModelPropertyBind],
            String, 'clazz',
            [ria.templates.ModelPropertyBind],
            Boolean, 'htmlText',
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.common.Button), 'buttons',
            [ria.templates.ModelPropertyBind],
            String, 'inputType',
            [ria.templates.ModelPropertyBind],
            String, 'inputValue',
            [ria.templates.ModelPropertyBind],
            Object, 'inputAttrs',
            [ria.templates.ModelPropertyBind],
            String, 'iconUrl'
        ])
});
