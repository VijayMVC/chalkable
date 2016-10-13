REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.recipients.GroupCreateTpl');

NAMESPACE('chlk.activities.recipients', function(){

    /**@class chlk.activities.recipients.GroupCreateDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.recipients.GroupCreateTpl)],
        'GroupCreateDialog', EXTENDS(chlk.activities.lib.TemplateDialog),[

            OVERRIDE, Object, function isReadyForClosing() {
                var selectedStudentsOnStart = this.dom.find('.selected-students-on-start').getValue(),
                    selectedStudents = this.dom.find('.selected-students').getValue(),
                    equal = true;

                selectedStudentsOnStart = selectedStudentsOnStart ? selectedStudentsOnStart.split(',') : [];
                selectedStudents = selectedStudents ? selectedStudents.split(',') : [];

                var selectedOnStart = selectedStudentsOnStart,
                    selected = selectedStudents;

                if(selectedOnStart.length != selected.length)
                    equal = false;
                else
                    selectedOnStart.forEach(function(item){
                        if(selected.indexOf(item) == -1)
                            equal = false;
                    });

                if (!equal) {
                    return this.view.ShowLeaveConfirmBox();
                }

                return true;
            }
        ]);
});