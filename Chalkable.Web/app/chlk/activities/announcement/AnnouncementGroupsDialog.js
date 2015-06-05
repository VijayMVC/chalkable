REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.group.AnnouncementGroupsTpl');
REQUIRE('chlk.templates.group.GroupExplorerTpl');
REQUIRE('chlk.templates.group.StudentsForGroupTpl');

NAMESPACE('chlk.activities.announcement', function(){

    /**@class chlk.activities.announcement.AnnouncementGroupsDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.group.AnnouncementGroupsTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.group.StudentsForGroupTpl, null, '.column.students' , ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.group.AnnouncementGroupsTpl, null, null , ria.mvc.PartialUpdateRuleActions.Replace)],
        'AnnouncementGroupsDialog', EXTENDS(chlk.activities.lib.TemplateDialog),[

            [ria.mvc.DomEventBind('click', '.add-groups')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function addGroupsClick(node, event){
                var ids = [];
                this.dom.find('.group-check').forEach(function(node){
                    if(node.checked())
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
                var allChecked = true;
                this.dom.find('.student-check').forEach(function(check){
                    if(!check.checked())
                        allChecked = false;
                });
                if(allCheck.checked() != allChecked)
                    allCheck.trigger(chlk.controls.CheckBoxEvents.CHANGE_VALUE.valueOf(), [allChecked]);
            },

            [ria.mvc.DomEventBind('change', '.grade-level-check')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function gradeLevelCheck(node, event, value_){
                var parent = node.parent('.school-grade-levels');
                var allCheck = parent.find('.school-check');
                var allChecked = true;
                parent.find('.grade-level-check').forEach(function(check){
                    if(!check.checked())
                        allChecked = false;
                });
                if(allCheck.checked() != allChecked)
                    allCheck.trigger(chlk.controls.CheckBoxEvents.CHANGE_VALUE.valueOf(), [allChecked]);
                this.schoolCheck();
            },

            [ria.mvc.DomEventBind('change', '.school-check')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function onSchoolCheck(node, event, value_){
                this.schoolCheck();
            },

            VOID, function schoolCheck(){
                var allCheck = this.dom.find('.all-schools-check');
                var allChecked = true;
                this.dom.find('.school-check').forEach(function(check){
                    if(!check.checked())
                        allChecked = false;
                });
                if(allCheck.checked() != allChecked)
                    allCheck.trigger(chlk.controls.CheckBoxEvents.CHANGE_VALUE.valueOf(), [allChecked]);
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.group.GroupExplorerTpl)],
            VOID, function updateGradeLevels(tpl, model, msg_) {
                tpl.renderTo(this.dom.find('.column.schools').setHTML(''));
                this.dom.find('.column.students').find('h1.column-cell').siblings().remove();
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.SuccessTpl)],
            VOID, function stopLoading(tpl, model, msg_) {

            }

        ]);
});