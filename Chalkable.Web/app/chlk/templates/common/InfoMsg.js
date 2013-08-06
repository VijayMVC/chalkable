REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.class.ClassForTopBar');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.CourseId');

NAMESPACE('chlk.templates.class', function () {

    /** @class chlk.templates.class.InfoMsg*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/common/InfoMsg.jade')],
        [ria.templates.ModelBind(chlk.models.common.InfoMsg)],
        'InfoMsg', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            String, 'text',
            [ria.templates.ModelPropertyBind],
            Array, 'buttonsInfo'
        ])
});