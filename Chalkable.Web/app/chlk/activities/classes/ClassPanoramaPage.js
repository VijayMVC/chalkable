REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.classes.ClassProfilePanoramaTpl');
REQUIRE('chlk.templates.classes.ClassProfilePanoramaChartsTpl');

NAMESPACE('chlk.activities.classes', function () {

    var studentsTimeout;

    /** @class chlk.activities.classes.ClassPanoramaPage */

    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.classes.ClassProfilePanoramaTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.classes.ClassProfilePanoramaChartsTpl, '', '.charts-part', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.classes.ClassProfilePanoramaTpl, '', null, ria.mvc.PartialUpdateRuleActions.Replace)],
        'ClassPanoramaPage', EXTENDS(chlk.activities.lib.TemplatePage), [

            [ria.mvc.PartialUpdateRule(null, 'save-filters')],
            VOID, function afterFiltersSave(tpl, model, msg_) {

            },

            [ria.mvc.DomEventBind('submit', '.class-panorama-form')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function formSubmit(node, event){
                var selectedStudents = [];
                this.dom.find('.student-check:checked').forEach(function(node){
                    selectedStudents.push(node.getData('id'));
                });

                var value = selectedStudents.length ? JSON.stringify(selectedStudents) : '';
                this.dom.find('.selected-students').setValue(value);
            },

            [ria.mvc.DomEventBind('change', '.all-students-check')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function allStudentsSelect(node, event, selected_){
                this.dom.find('.student-check').forEach(function(element){
                    element.trigger(chlk.controls.CheckBoxEvents.CHANGE_VALUE.valueOf(), [node.is(':checked')]);
                });
            },

            [ria.mvc.DomEventBind('change', '.student-grid-check')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function studentsChange(node, event, selected_){
                clearTimeout(studentsTimeout);

                studentsTimeout = setTimeout(function(){
                    node.parent('form').trigger('submit');
                }, 500)
            },

            [ria.mvc.DomEventBind(chlk.controls.TabEvents.TAB_CHANGED.valueOf(), '.tabs-block')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function lessStandardsClick(node, event){
                this.updateChartsSize_();
            },

            [[Object]],
            OVERRIDE, VOID, function onRefresh_(model) {
                BASE(model);
                this.updateChartsSize_();
            },

            [[Object, String]],
            OVERRIDE, VOID, function onPartialRefresh_(model, msg_) {
                BASE(model, msg_);
                this.updateChartsSize_();
            },

            function updateChartsSize_(){
                jQuery(window).trigger('resize');
            }
        ]);
});