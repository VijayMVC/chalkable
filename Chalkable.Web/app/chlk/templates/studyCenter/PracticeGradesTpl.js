REQUIRE('chlk.templates.common.PageWithClasses');
REQUIRE('chlk.models.studyCenter.PracticeGradesViewData');

NAMESPACE('chlk.templates.studyCenter', function(){
    "use strict";

    /**@class chlk.templates.student.PracticeGradesTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/studyCenter/PracticeGradesView.jade')],
        [ria.templates.ModelBind(chlk.models.studyCenter.PracticeGradesViewData)],
        'PracticeGradesTpl', EXTENDS(chlk.templates.common.PageWithClasses),[
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.standard.Standard), 'standards',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.studyCenter.PracticeGradeViewData), 'practiceGrades',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassId, 'classId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.SchoolPersonId, 'studentId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.StandardId, 'standardId',

            [ria.templates.ModelPropertyBind],
            Boolean, 'startPractice',

            chlk.models.common.ActionLinkModel, function createActionLinkModel_(controller, action, title
                , pressedAction_, args_, disabled_){
                return new chlk.models.common.ActionLinkModel(controller, action, title
                    , pressedAction_ && pressedAction_ == action, args_, null, disabled_);
            },

            [[String]],
            ArrayOf(chlk.models.common.ActionLinkModel), function buildActionLinkModels(pressedLinkName){
                var controller = 'studycenter';
                var userId = this.getCurrentUser().getId().valueOf();
                var res = [this.createActionLinkModel_(controller, 'explorer', 'Explorer', pressedLinkName, [], !this.isStudyCenterEnabled()),
                    this.createActionLinkModel_(controller, 'practice', 'Practice', pressedLinkName, [null], !this.isStudyCenterEnabled())
                ];
                return res;
            },

            function getCurrentStandardIds(){
                var standardId = this.getStandardId();
                if (standardId && standardId.valueOf())
                    return [this.getStandardId()];

                return this.getStandards().map(function f(item){
                    return item.getStandardId();
                });
            }
        ]);
});