REQUIRE('chlk.models.common.ServerErrorModel');
REQUIRE('chlk.models.common.PageWithClasses');

NAMESPACE('chlk.models.common', function(){
    "use strict";
    /**@class chlk.models.common.ServerErrorWithClassesModel*/

    CLASS('ServerErrorWithClassesModel', EXTENDS(chlk.models.common.PageWithClasses), [

        chlk.models.common.ServerErrorModel, 'error',

        String, 'controller',

        String, 'action',

        Array, 'params',

        [[chlk.models.classes.ClassesForTopBar, chlk.models.id.ClassId, chlk.models.common.ServerErrorModel]],
        function $(classes_, classId_, serverError_){
            BASE(classes_, classId_);
            if(serverError_){
                this.setError(serverError_)
            }
        },

        [[chlk.models.classes.ClassesForTopBar, String, String, String, Array]],
        function $create(classes, message, controller_, action_, params_){
            BASE(classes, null);
            if(message){
                var serverError = new chlk.models.common.ServerErrorModel(message);
                this.setError(serverError);
            }
            if(controller_)
                this.setController(controller_);
            if(action_)
                this.setAction(action_);
            if(params_)
                this.setParams(params_);
        }
    ]);
});