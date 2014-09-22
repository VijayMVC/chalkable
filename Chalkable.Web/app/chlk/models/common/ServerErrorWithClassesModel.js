REQUIRE('chlk.models.common.ServerErrorModel');
REQUIRE('chlk.models.common.PageWithClasses');

NAMESPACE('chlk.models.common', function(){
    "use strict";
    /**@class chlk.models.common.ServerErrorWithClassesModel*/

    CLASS('ServerErrorWithClassesModel', EXTENDS(chlk.models.common.PageWithClasses), [

        chlk.models.common.ServerErrorModel, 'error',

        [[chlk.models.classes.ClassesForTopBar, chlk.models.id.ClassId, chlk.models.common.ServerErrorModel]],
        function $(classes_, classId_, serverError_){
            BASE(classes_, classId_);
            if(serverError_){
                this.setError(serverError_)
            }
        },

        [[chlk.models.classes.ClassesForTopBar, String]],
        function $create(classes, message){
            BASE(classes, null);
            if(message){
                var serverError = new chlk.models.common.ServerErrorModel(message);
                this.setError(serverError);
            }
        }
    ]);
});