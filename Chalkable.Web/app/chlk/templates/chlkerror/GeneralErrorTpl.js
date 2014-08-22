REQUIRE('chlk.models.common.ServerErrorModel');
REQUIRE('chlk.templates.common.PageWithClasses');

NAMESPACE('chlk.templates.chlkerror',function(){
    "use strict";
    /**@class chlk.templates.chlkerror.GeneralErrorTpl*/

    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/chlkerror/GeneralErrorPage.jade')],
        [ria.templates.ModelBind(chlk.models.common.ServerErrorModel)],
        'GeneralErrorTpl', EXTENDS(chlk.templates.ChlkTemplate),[

            [ria.templates.ModelPropertyBind],
            String, 'message',

            [ria.templates.ModelPropertyBind],
            String, 'stackTrace',

            function getText(){
                return this.getMessage();
            }
        ]);
});