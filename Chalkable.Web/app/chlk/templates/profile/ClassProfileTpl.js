REQUIRE('chlk.templates.profile.BaseProfileTpl');
REQUIRE('chlk.models.common.ActionLinkModel');

NAMESPACE('chlk.templates.profile', function(){
    "use strict";

    /**@class chlk.templates.profile.ClassProfileTpl*/
    CLASS(
        [ria.templates.ModelBind(chlk.models.classes.Class)],
        'ClassProfileTpl', EXTENDS(chlk.templates.profile.BaseProfileTpl),[

            [[String, chlk.models.id.ClassId]],
            OVERRIDE, ArrayOf(chlk.models.common.ActionLinkModel), function buildActionLinkModels(pressedActionName, classId_){

                if(!classId_ && (this.getModel() instanceof chlk.models.classes.Class)){
                    classId_ = this.getModel().getId();
                }
                return [
                    this.buildActionLinkModelForClass('details', 'Now', pressedActionName, classId_),
                    this.buildActionLinkModelForClass('info', 'Info', pressedActionName, classId_),
                    this.buildActionLinkModelForClass('schedule', 'Schedule', pressedActionName, classId_),
                    this.buildActionLinkModelForClass('grading', 'Grading', pressedActionName, classId_),
                    this.buildActionLinkModelForClass('attendance', 'Attendance', pressedActionName, classId_),
                    this.buildActionLinkModelForClass('apps', 'Apps', pressedActionName, classId_)
                ]
            },

            [[String, String, String, chlk.models.id.ClassId]],
            chlk.models.common.ActionLinkModel, function buildActionLinkModelForClass(action, title, pressedActionName, classId_){
                return new chlk.models.common.ActionLinkModel('class', action, title, pressedActionName == action, [classId_ || null]);
            }
    ]);
});