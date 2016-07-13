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
        'AnnouncementGroupsDialog', EXTENDS(chlk.activities.lib.TemplateDialog),[

            [ria.mvc.PartialUpdateRule(chlk.templates.group.StudentsForGroupTpl)],
            VOID, function updateStudents(tpl, model, msg_) {
                tpl.renderTo(this.dom.find('.column.students').setHTML('').removeClass('filtered'));
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.group.AnnouncementGroupsTpl)],
            VOID, function updateDialog(tpl, model, msg_) {
                var selected = [];
                this.dom.find('.group-check:checked').forEach(function(checkbox){
                    selected.push(new chlk.models.id.GroupId(parseInt(checkbox.getData('id'), 10)));
                });
                model.setSelected(selected);
                tpl.setSelected(selected);
                tpl.renderTo(this.dom.setHTML(''));
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.group.StudentsForGroupTpl, 'after-filter')],
            VOID, function updateStudentsAfterFilter(tpl, model, msg_) {
                tpl.renderTo(this.dom.find('.column.students').setHTML('').addClass('filtered'));
            },

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
                if(this.dom.find('.column.students').hasClass('filtered'))
                    return false;

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
                var checked = 0, allChecked = true, partially = 0;
                checkBoxes.forEach(function(check){
                    if(!check.checked())
                        allChecked = false;
                    else{
                        if(check.hasClass('partially-checked'))
                            partially++;
                        else
                            checked++;
                    }

                });
                if(!checked && !partially){
                    allCheck.removeClass('partially-checked');

                    if(allCheck.checked())
                        allCheck.trigger(chlk.controls.CheckBoxEvents.CHANGE_VALUE.valueOf(), [false]);
                }
                else {
                    if(!allChecked || partially){
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
                if(allSchools.checked()){
                    groupCheck.removeAttr('disabled');
                    groupCheck.parent('.box-checkbox').removeClass('disabled');
                }else{
                    groupCheck.setAttr('disabled', 'disabled');
                    groupCheck.parent('.box-checkbox').addClass('disabled');
                }
                this.checkSubmitButton();
            },

            [[Object, String]],
            OVERRIDE, VOID, function onPartialRefresh_(model, msg_) {
                BASE(model, msg_);
                if(currentGradeLevel)
                    currentGradeLevel.trigger('click');
                currentGradeLevel = undefined;
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.group.GroupExplorerTpl)],
            VOID, function updateGradeLevels(tpl, model, msg_) {
                tpl.renderTo(this.dom.find('.column.schools').setHTML(''));
                this.dom.find('.column.students').find('h1.column-cell').siblings().remove();
                this.dom.find('.filter-button').remove();
                this.schoolCheck();
            },

            function setCheckBoxValue(node, value){
                var jNode = jQuery(node);
                jNode.prop('checked', value);
                value ? node.setAttribute('checked', 'checked') : node.removeAttribute('checked');
                value && node.setAttribute('checked', 'checked');
                var hidden = jNode.parent().find('.hidden-checkbox');
                hidden.val(value);
                hidden.data('value', value);
                hidden.attr('data-value', value);
            },

            [ria.mvc.DomEventBind('change', '.all-students-check')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function allStudentsChange(node, event, value_){
                var value = node.checked(), that = this;
                jQuery(node.valueOf()).parents('.column').find('.students').find('input[type=checkbox]').each(function(index, item){
                    that.setCheckBoxValue(item, value);
                    jQuery(item).removeClass('partially-checked');
                });
                this.updateGradeLevel();
            },

            [ria.mvc.DomEventBind('change', '.all-schools-check')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function allSchoolsChange(node, event, value_){
                var value = node.checked(), that = this;
                jQuery(node.valueOf()).parents('.column').find('.school-grade-levels').find('input[type=checkbox]').each(function(index, item){
                    that.setCheckBoxValue(item, value);
                    jQuery(item).removeClass('partially-checked');
                });
                currentGradeLevel = this.dom.find('.grade-level-name.active');
                this.prepareGroupCheck();
            },

            [ria.mvc.DomEventBind('change', '.school-check')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function schoolChange(node, event, value_){
                var value = node.checked(), that = this;
                jQuery(node.valueOf()).parents('.school-grade-levels').find('.grade-level-name').find('input[type=checkbox]').each(function(index, item){
                    that.setCheckBoxValue(item, value);
                    jQuery(item).removeClass('partially-checked');
                });
                currentGradeLevel = this.dom.find('.grade-level-name.active');
                this.schoolCheck();
            },

            [ria.mvc.DomEventBind('change', '.group-check')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function groupChange(node, event, value_){
                this.checkSubmitButton();
            },

            function checkSubmitButton(){
                var disabled = this.dom.find('.group-check:checked:not(:disabled)').count() == 0;
                this.dom.find('.add-groups').setProp('disabled', disabled).removeClass('disabled');
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.SuccessTpl)],
            VOID, function stopLoading(tpl, model, msg_) {

            }

        ]);
});