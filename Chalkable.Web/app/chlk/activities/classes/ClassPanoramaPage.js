REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.classes.ClassProfilePanoramaTpl');
REQUIRE('chlk.templates.classes.ClassProfilePanoramaChartsTpl');
REQUIRE('chlk.templates.classes.ClassProfilePanoramaTestsChartTpl');
REQUIRE('chlk.templates.classes.ClassProfilePanoramaStudentsTpl');

NAMESPACE('chlk.activities.classes', function () {

    var studentsTimeout, disableUpdateByColumn;

    /** @class chlk.activities.classes.ClassPanoramaPage */

    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.classes.ClassProfilePanoramaTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.classes.ClassProfilePanoramaChartsTpl, '', '.charts-part', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.classes.ClassProfilePanoramaStudentsTpl, 'sort', '.panorama-students-tab', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.classes.ClassProfilePanoramaTestsChartTpl, chlk.activities.lib.DontShowLoader(), '.standardized-tests-tab', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.classes.ClassProfilePanoramaTpl, '', null, ria.mvc.PartialUpdateRuleActions.Replace)],
        'ClassPanoramaPage', EXTENDS(chlk.activities.lib.TemplatePage), [

            [ria.mvc.PartialUpdateRule(null, 'save-filters')],
            VOID, function afterFiltersSave(tpl, model, msg_) {

            },

            function clearCheckedStudents_(){
                this.dom.find('.student-grid-check:checked').trigger(chlk.controls.CheckBoxEvents.CHANGE_VALUE.valueOf(), [false]);
                this.dom.find('.selected-students').setValue('');
                this.dom.find('.add-supplemental-btn').addClass('v-hidden');
            },

            function clearHighlightedStudents_(){
                this.dom.find('.highlighted-students').setValue('');
                this.dom.find('.students-grid').find('.highlighted').removeClass('highlighted');
                disableUpdateByColumn = true;

                jQuery('.distribution-chart').each(function(){
                    var chart = jQuery(this).highcharts();
                    chart.series[0].data.forEach(function(item){
                        item.select(false);
                    });
                });

                setTimeout(function(){
                    disableUpdateByColumn = false;
                }, 1);
            },

            [ria.mvc.DomEventBind('columnselect', '.distribution-chart')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function columnSelect(node, event, selected_){
                if(!disableUpdateByColumn){
                    clearTimeout(studentsTimeout);
                    var grid = this.dom.find('.students-grid'), dom = this.dom, currentChart = node.valueOf()[0];

                    this.clearCheckedStudents_();

                    grid.find('.highlighted').removeClass('highlighted');

                    selected_ && selected_.forEach(function(id){
                        grid.find('.grid-row[data-id=' + id + ']').addClass('highlighted');
                    });

                    studentsTimeout = setTimeout(function(){
                        var value = selected_ && selected_.length ? JSON.stringify(selected_) : '';
                        dom.find('.highlighted-students').setValue(value);

                        node.parent('form').find('.submit-by-check').trigger('click');
                        dom.find('.standardized-tests-tab').addClass('partial-update');
                    }, 1000);


                    disableUpdateByColumn = true;

                    jQuery('.distribution-chart').each(function(){
                        if(!jQuery(this).is(currentChart)){
                            var chart = jQuery(this).highcharts();
                            chart.series[0].data.forEach(function(item){
                                item.select(false);
                            });
                        }
                    });

                    setTimeout(function(){
                        disableUpdateByColumn = false;
                    }, 1);
                }

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

                this.clearHighlightedStudents_();

                var btn = this.dom.find('.add-supplemental-btn'),
                    dom = this.dom;

                setTimeout(function(){
                    if(dom.find('.student-check:checked').count())
                        btn.removeClass('v-hidden');
                    else
                        btn.addClass('v-hidden');
                }, 1);

                studentsTimeout = setTimeout(function(){
                    var selectedStudents = [];
                    dom.find('.student-check:checked').forEach(function(node){
                        selectedStudents.push(node.getData('id'));
                    });

                    var value = selectedStudents.length ? JSON.stringify(selectedStudents) : '';
                    dom.find('.selected-students').setValue(value);

                    node.parent('form').find('.submit-by-check').trigger('click');
                    dom.find('.standardized-tests-tab').addClass('partial-update');
                }, 1000)

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
                if(msg_ == chlk.activities.lib.DontShowLoader())
                    this.dom.find('.standardized-tests-tab').removeClass('partial-update');

                if(msg_ == 'sort'){
                    var selectedStudents = this.dom.find('.selected-students').getValue(), dom = this.dom;

                    if(selectedStudents){
                        selectedStudents = JSON.parse(selectedStudents);

                        selectedStudents.forEach(function(id){
                            dom.find('.student-check[data-id=' + id + ']').trigger(chlk.controls.CheckBoxEvents.CHANGE_VALUE.valueOf(), [true]);
                        });
                    }else{
                        var highlightedStudents = this.dom.find('.highlighted-students').getValue();

                        if(highlightedStudents){
                            highlightedStudents = JSON.parse(highlightedStudents);

                            highlightedStudents.forEach(function(id){
                                dom.find('.grid-row[data-id=' + id + ']').addClass('highlighted');
                            });
                        }
                    }
                }
            },

            function updateChartsSize_(){
                jQuery(window).trigger('resize');
            }
        ]);
});