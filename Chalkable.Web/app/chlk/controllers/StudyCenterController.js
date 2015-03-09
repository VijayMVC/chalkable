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
        chlk.services.StandardService, 'standardService',

        [ria.mvc.Inject],
        chlk.services.StudyCenterService, 'studyCenterService',

        [chlk.controllers.SidebarButton('study-center')],
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
        [chlk.controllers.SidebarButton('study-center')],
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
        [chlk.controllers.SidebarButton('study-center')],
        [[Object, Boolean, chlk.models.id.StandardId]],
        function startPracticeAction(standardId, changeStandard_, currentId_){
            if (!Array.isArray(standardId))
                standardId = [standardId];

            if(!standardId.length){
                this.BackgroundCloseView(chlk.activities.apps.MiniQuizDialog);
                return null;
            }

            standardId = standardId.map(chlk.models.id.StandardId);

            var res = ria.async.wait(
                    this.studyCenterService.getMiniQuizInfo(currentId_ || standardId[0]).attach(this.validateResponse_()),
                    this.standardService.getStandardsList(standardId).attach(this.validateResponse_())
                ).then(function(models){
                    models[0].setCurrentStandardId(currentId_ || standardId[0]);
                    models[0].setStandards(models[1]);
                    return models[0];
                });

            if(changeStandard_)
                return this.UpdateView(chlk.activities.apps.MiniQuizDialog, res, 'content');
            return this.ShadeOrUpdateView(chlk.activities.apps.MiniQuizDialog, res);
        },

        [chlk.controllers.StudyCenterEnabled],
        [chlk.controllers.SidebarButton('study-center')],
        [[chlk.models.apps.MiniQuizViewData]],
        function updateMiniQuizAction(model){
            var standardIds = JSON.parse(model.getStandardIds());
            return this.Redirect('studycenter', 'startPractice', [standardIds, null, model.getCurrentStandardId().valueOf() ? model.getCurrentStandardId() : null]);
        },

        [[chlk.models.studyCenter.PracticeGradesViewData]],
        function filterPracticeGradesByStandardIdAction(model){
            return this.Redirect('studycenter', 'practice', [model.getClassId(), model.getStandardId()]);
        }
    ])
});
