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
                var res = [this.createActionLinkModel_(controller, 'explorer', 'Explorer', pressedLinkName, [userId], !this.isStudyCenterEnabled()),
                    this.createActionLinkModel_(controller, 'practice', 'Practice', pressedLinkName, [null, userId], !this.isStudyCenterEnabled())
                ];
                return res;
            },

            function getCurrentCode(){
                var standardId = this.getStandardId();
                if(standardId && standardId.valueOf())
                    return this.getStandards().filter(function(item){return item.getStandardId() == standardId})[0].getCommonCoreStandardCode();

                var filerFunction = function(item){
                    return item.getCommonCoreStandardCode();
                };

                return this.getStandards().filter(filerFunction).map(filerFunction);

            }
        ]);
});