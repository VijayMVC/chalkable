REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.StudentService');
REQUIRE('chlk.services.StudyCenterService');

REQUIRE('chlk.activities.student.StudentExplorerPage');
REQUIRE('chlk.activities.studyCenter.PracticeGradesPage');
REQUIRE('chlk.activities.apps.MiniQuizDialog');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.StudyCenterController*/
    CLASS(
        'StudyCenterController', EXTENDS(chlk.controllers.BaseController), [

        [ria.mvc.Inject],
        chlk.services.StudentService, 'studentService',

        [ria.mvc.Inject],
        chlk.services.StudyCenterService, 'studyCenterService',

        [chlk.controllers.SidebarButton('play')],
        [chlk.controllers.StudyCenterEnabled],
        function explorerAction(){
            var personId = this.getCurrentPerson().getId();
            var res = this.studentService.getExplorer(personId)
                .attach(this.validateResponse_())
                .then(function(studentExplorer){
                    return new chlk.models.student.StudentExplorerViewData(
                        this.getCurrentRole(), studentExplorer, this.getUserClaims_(), 'Study Center'
                    )
                }, this);
            return this.PushView(chlk.activities.student.StudentExplorerPage, res);
        },

        [chlk.controllers.StudyCenterEnabled],
        [chlk.controllers.SidebarButton('play')],
        [[chlk.models.id.ClassId, chlk.models.id.StandardId]],
        function practiceAction(classId_, standardId_){
            var personId = this.getCurrentPerson().getId(), res;
            if(!classId_ || !classId_.valueOf())
                res = ria.async.DeferredData(new chlk.models.studyCenter.PracticeGradesViewData(new chlk.models.classes.ClassesForTopBar(null)));
            else
                res = this.studyCenterService.getPracticeGrades(personId, classId_, standardId_)
                    .attach(this.validateResponse_())
                    .then(function(model){
                        model.setTopData(new chlk.models.classes.ClassesForTopBar(null, classId_));
                        standardId_ && model.setStandardId(standardId_);
                        return model;
                    });
            return this.PushOrUpdateView(chlk.activities.studyCenter.PracticeGradesPage, res);
        },

        [chlk.controllers.StudyCenterEnabled],
        [chlk.controllers.SidebarButton('play')],
        [[Object]],
        function startPracticeAction(ccStandardCode){
            if (!Array.isArray(ccStandardCode))
                ccStandardCode = [ccStandardCode];

            var res = this.studyCenterService
                .getMiniQuizInfo(ccStandardCode[0])
                .attach(this.validateResponse_())
                .then(function(model){
                    model.setCurrentStandardCode(ccStandardCode[0]);
                    model.setCcStandardCodes(ccStandardCode);
                    return model;
                });

            return this.ShadeView(chlk.activities.apps.MiniQuizDialog, res);
        },

        [[chlk.models.studyCenter.PracticeGradesViewData]],
        function filterPracticeGradesByStandardIdAction(model){
            return this.Redirect('studycenter', 'practice', [model.getClassId(), model.getStandardId()]);
        }
    ])
});
