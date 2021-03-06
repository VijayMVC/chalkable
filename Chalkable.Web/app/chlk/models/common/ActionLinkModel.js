NAMESPACE('chlk.models.common', function(){
    "use strict";
    /**@class chlk.models.common.ActionLinkModel*/
    CLASS('ActionLinkModel', [

        String, 'controllerName',
        String, 'actionName',
        Boolean, 'pressed',
        Boolean, 'disabled',
        ArrayOf(Object), 'args',
        String, 'title',
        String, 'sisApiVersion',


        ArrayOf(String), 'classesNames',

        [[String, String, String, Boolean, ArrayOf(Object), ArrayOf(String), Boolean, String]],
        function $(controllerName, actionName, title_, isPressed_, args_, classesNames_, disabled_, sisApiVersion_){
            BASE();
            this.setControllerName(controllerName);
            this.setActionName(actionName);
            this.setPressed(false);
            this.setDisabled(false);
            if(sisApiVersion_)
                this.setSisApiVersion(sisApiVersion_);
            if(args_)
                this.setArgs(args_);
            if(title_)
                this.setTitle(title_);
            if(isPressed_){
                this.setPressed(isPressed_);
                classesNames_ = (classesNames_ || []).concat(['pressed']);
            }
            if(disabled_){
                this.setDisabled(disabled_);
                classesNames_ = (classesNames_ || []).concat(['disabled']);
            }
            if(classesNames_)
                this.setClassesNames(classesNames_);
        }
    ]);
});