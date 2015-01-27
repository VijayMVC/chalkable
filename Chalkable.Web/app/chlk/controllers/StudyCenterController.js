REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.StudentService');

REQUIRE('chlk.activities.student.StudentExplorerPage');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.StudyCenterController*/
    CLASS(
        'StudyCenterController', EXTENDS(chlk.controllers.BaseController), [

        [ria.mvc.Inject],
        chlk.services.StudentService, 'studentService',

        [chlk.controllers.SidebarButton('play')],
        [chlk.controllers.StudyCenterEnabled()],
        [[chlk.models.id.SchoolPersonId]],
        function explorerAction(personId_){
            var personId = personId_ || this.getCurrentPerson().getId();
            var res = this.studentService.getExplorer(personId)
                .attach(this.validateResponse_())
                .then(function(studentExplorer){
                    return new chlk.models.student.StudentExplorerViewData(
                        this.getCurrentRole(), studentExplorer, this.getUserClaims_()
                    )
                }, this);
            return this.PushView(chlk.activities.student.StudentExplorerPage, res);
        },

        [chlk.controllers.StudyCenterEnabled()],
        [[chlk.models.id.SchoolPersonId]],
        function practiceAction(personId_){
            return null;
        }
    ])
});
