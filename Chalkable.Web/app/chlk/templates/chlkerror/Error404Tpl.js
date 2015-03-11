REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.Success');

NAMESPACE('chlk.templates.chlkerror', function () {
    "use strict";
    /** @class chlk.templates.chlkerror.Error404Tpl*/

    CLASS(
        [ria.templates.ModelBind(chlk.models.Success)],
        [ria.templates.TemplateBind('~/assets/jade/activities/chlkerror/Error404Page.jade')],
        'Error404Tpl', EXTENDS(chlk.templates.ChlkTemplate), []);
});