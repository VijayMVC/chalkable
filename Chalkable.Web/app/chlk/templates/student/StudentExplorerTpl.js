REQUIRE('chlk.templates.student.StudentProfileExplorerTpl');
REQUIRE('chlk.models.student.StudentExplorerViewData');

NAMESPACE('chlk.templates.student', function(){
    "use strict";

    /**@class chlk.templates.student.StudentExplorerTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/student/StudentProfileExplorerView.jade')],
        [ria.templates.ModelBind(chlk.models.student.StudentExplorerViewData)],
        'StudentExplorerTpl', EXTENDS(chlk.templates.student.StudentProfileExplorerTpl),[
            [[String]],
            OVERRIDE, ArrayOf(chlk.models.common.ActionLinkModel), function buildActionLinkModels(pressedLinkName){
                var controller = 'studycenter';
                var userId = this.getUser().getId().valueOf();
                var res = [this.createActionLinkModel_(controller, 'explorer', 'Explorer', pressedLinkName, [], !this.isStudyCenterEnabled()),
                    this.createActionLinkModel_(controller, 'practice', 'Practice', pressedLinkName, [null], !this.isStudyCenterEnabled())
                ];
                return res;
            }
        ]);
});