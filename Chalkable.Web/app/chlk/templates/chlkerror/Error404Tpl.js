REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.Success');

NAMESPACE('chlk.templates.chlkerror', function () {
    "use strict";

    /** @class chlk.templates.chlkerror.Error404Model*/
    CLASS(
        'Error404Model', [
            String, 'msg',

            function $withMsg(msg) {
                BASE();
                this.msg = msg;
            }
        ]);

    /** @class chlk.templates.chlkerror.Error404Tpl*/
    CLASS(
        [ria.templates.ModelBind(chlk.templates.chlkerror.Error404Model)],
        [ria.templates.TemplateBind('~/assets/jade/activities/chlkerror/Error404Page.jade')],
        'Error404Tpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            String, 'msg'
        ]);
});
