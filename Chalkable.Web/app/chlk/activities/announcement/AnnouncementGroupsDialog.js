REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.group.AnnouncementGroupsTpl');
REQUIRE('chlk.templates.group.GroupExplorerTpl');
REQUIRE('chlk.templates.group.StudentsForGroupTpl');

NAMESPACE('chlk.activities.announcement', function(){

    var currentGradeLevel;

    /**@class chlk.activities.announcement.AnnouncementGroupsDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.group.AnnouncementGroupsTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.group.AnnouncementGroupsTpl, null, null , ria.mvc.PartialUpdateRuleActions.Replace)],
        'AnnouncementGroupsDialog', EXTENDS(chlk.activities.lib.TemplateDialog),[

            [ria.mvc.DomEventBind('click', '.add-groups')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function addGroupsClick(node, event){
                var ids = [];
                this.dom.find('.group-check').forEach(function(node){
                    if(!node.is(':disabled') && node.checked())
                        ids.push(node.getData('id'))
                });
                this.dom.find('.group-ids').setValue(ids.join(','));
            },

            [ria.mvc.DomEventBind('click', '.action-checkbox')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function actionCheckboxClick(node, event){
                this.dom.find('.saved-block').fadeIn();
            },

            [ria.mvc.DomEventBind('click', '.name:not(.active)')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function nameClick(node, event){
                node.parent('td').find('.active').removeClass('active');
                node.addClass('active');
            },

            [ria.mvc.DomEventBind('change', '.student-check')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function studentCheck(node, event, value_){
                var allCheck = this.dom.find('.all-students-check');
                this.prepareCheckBoxes(this.dom.find('.student-check'), allCheck);
                this.updateGradeLevel();
            },

            function updateGradeLevel(){
                var allCheck = this.dom.find('.all-students-check');
                var gradeLevelCheck = this.dom.find('.grade-level-name.active').parent().find('input[type=checkbox]');
                if(allCheck.checked() != gradeLevelCheck.checked())
                    gradeLevelCheck.trigger(chlk.controls.CheckBoxEvents.CHANGE_VALUE.valueOf(), [allCheck.checked()]);
                if(allCheck.hasClass('partially-checked'))
                    gradeLevelCheck.addClass('partially-checked');
                else
                    gradeLevelCheck.removeClass('partially-checked');
                this.afterGradeLevelChange(gradeLevelCheck);
            },

            function afterGradeLevelChange(node){
                var parent = node.parent('.school-grade-levels');
                this.prepareCheckBoxes(parent.find('.grade-level-check'), parent.find('.school-check'));
                this.schoolCheck();
            },

            [ria.mvc.DomEventBind('change', '.grade-level-check')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function gradeLevelCheck(node, event, value_){
                this.afterGradeLevelChange(node);
                var link = node.parent('.column-cell:eq(0)').find('.grade-level-name');
                if(link.hasClass('active'))
                    currentGradeLevel = link;
            },

            function prepareCheckBoxes(checkBoxes, allCheck){
                var checked = 0, allChecked = true;
                checkBoxes.forEach(function(check){
                    if(!check.checked())
                        allChecked = false;
                    else
                        checked++;
                });
                if(!checked){
                    allCheck.removeClass('partially-checked');

                    if(allCheck.checked())
                        allCheck.trigger(chlk.controls.CheckBoxEvents.CHANGE_VALUE.valueOf(), [false]);
                }
                else {
                    if(!allChecked){
                        allCheck.trigger(chlk.controls.CheckBoxEvents.CHANGE_VALUE.valueOf(), [true]);
                        allCheck.addClass('partially-checked');
                    }
                    else
                        allCheck.removeClass('partially-checked');

                        if(!allCheck.checked())
                            allCheck.trigger(chlk.controls.CheckBoxEvents.CHANGE_VALUE.valueOf(), [true]);
                }

            },

            VOID, function schoolCheck(){
                this.prepareCheckBoxes(this.dom.find('.school-check'), this.dom.find('.all-schools-check'));
                this.prepareGroupCheck();
            },

            function prepareGroupCheck(){
                var allSchools = this.dom.find('.all-schools-check'),
                    groupCheck = this.dom.find('.group-name.active').parent().find('input[type=checkbox]');
                if(allSchools.checked() || allSchools.hasClass('partially-checked')){
                    groupCheck.removeAttr('disabled');
                    groupCheck.parent('.box-checkbox').removeClass('disabled');
                }else{
                    groupCheck.setAttr('disabled', 'disabled');
                    groupCheck.parent('.box-checkbox').addClass('disabled');
                }
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.group.StudentsForGroupTpl)],
            VOID, function updateStudents(tpl, model, msg_) {
                tpl.renderTo(this.dom.find('.column.students').setHTML(''));
                if(currentGradeLevel)
                    currentGradeLevel.trigger('click');
                currentGradeLevel = undefined;
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.group.StudentsForGroupTpl, 'all-students')],
            VOID, function updateAllStudents(tpl, model, msg_) {
                tpl.renderTo(this.dom.find('.column.students').setHTML(''));
                this.updateGradeLevel();
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.group.GroupExplorerTpl)],
            VOID, function updateGradeLevels(tpl, model, msg_) {
                tpl.renderTo(this.dom.find('.column.schools').setHTML(''));
                this.dom.find('.column.students').find('h1.column-cell').siblings().remove();
                this.dom.find('.filter-button').remove();
                this.schoolCheck();
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.SuccessTpl)],
            VOID, function stopLoading(tpl, model, msg_) {

            }

        ]);
});