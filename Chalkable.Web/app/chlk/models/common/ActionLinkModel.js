NAMESPACE('chlk.models.common', function(){
    "use strict";
    /**@class chlk.models.common.ActionLinkModel*/
    CLASS('ActionLinkModel', [

        String, 'controllerName',
        String, 'actionName',
        Boolean, 'pressed',
        ArrayOf(Object), 'args',
        String, 'title',

        ArrayOf(String), 'classesNames',

        [[String, String, String, Boolean, ArrayOf(Object), ArrayOf(String)]],
        function $(controllerName, actionName, title_, isPressed_, args_, classesNames_){
            BASE();
            this.setControllerName(controllerName);
            this.setActionName(actionName);
            if(isPressed_)
                this.setPressed(isPressed_);
            if(args_)
                this.setArgs(args_);
            if(title_)
                this.setTitle(title_);
            if(classesNames_)
                this.setClassesNames(classesNames_);
        }
    ]);
});