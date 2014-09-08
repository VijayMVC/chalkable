REQUIRE('chlk.models.common.ServerErrorWithClassesModel');
REQUIRE('chlk.templates.common.PageWithClasses');
REQUIRE('chlk.templates.chlkerror.GeneralErrorTpl');

NAMESPACE('chlk.templates.chlkerror',function(){
    "use strict";
    /**@class chlk.templates.chlkerror.GeneralServerErrorWithClassesTpl*/

    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/chlkerror/GeneralServerErrorWithClasses.jade')],
        [ria.templates.ModelBind(chlk.models.common.ServerErrorWithClassesModel)],
        'GeneralServerErrorWithClassesTpl', EXTENDS(chlk.templates.common.PageWithClasses),[

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ServerErrorModel, 'error'
        ]);
});
