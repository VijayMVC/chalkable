REQUIRE('chlk.templates.grading.FinalGradesTpl');
REQUIRE('chlk.templates.grading.GradingPeriodFinalGradeTpl');
REQUIRE('chlk.templates.grading.FinalGradeStudentBlockTpl');

REQUIRE('chlk.activities.lib.TemplatePage');

NAMESPACE('chlk.activities.grading', function () {

    var slideTimeout;

    /** @class chlk.activities.grading.FinalGradesPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.grading.FinalGradesTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.grading.GradingPeriodFinalGradeTpl, 'average-change', '.big-grading-period-container.open')],
        'FinalGradesPage', EXTENDS(chlk.activities.lib.TemplatePage), [
            [[ria.dom.Dom, Boolean]],
            VOID, function changeLineOpacity(node, needOpacity){
                var color = needOpacity ? 'rgba(193,193,193,0.5)' : node.getData('color');
                var index = node.getData('index');
                var chart = this.dom.find('.main-chart:visible').getData('chart');
                chart.series[index].graph && chart.series[index].graph.attr({ stroke: color });
                chart.series[index].update(this.getSerieConfigs_(!needOpacity, color));
            },

            [[Boolean, String]],
            function getSerieConfigs_(enabled, color_){
                return enabled ? {
                    marker : {
                        enabled: true,
                        symbol: 'circle',
                        radius: 3,
                        fillColor: '#ffffff',
                        lineWidth: 2,
                        lineColor: color_,
                        states: {
                            hover: {
                                radius: 6,
                                lineWidth: 2,
                                enabled: true
                            }
                        }
                    },
                    color: color_,
                    zIndex: 10,
                    enableMouseTracking: true
                } : {
                    marker : {
                        enabled: false,
                        states: {
                            hover: {
                                enabled: false
                            }
                        }
                    },
                    color: "#c1c1c1",
                    zIndex: 1,
                    enableMouseTracking: true
                };
            },

            [ria.mvc.DomEventBind('mouseover mouseleave', '.legend-item')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function itemTypeHover(node, event){
                var needOpacity = event.type == 'mouseleave';
                this.changeLineOpacity(node, needOpacity);
            },

            [ria.mvc.DomEventBind('seriemouseover seriemouseleave', '.main-chart')],
            [[ria.dom.Dom, ria.dom.Event, Object, Object]],
            VOID, function chartHover(node, event, chart_, hEvent_){
                var needOpacity = event.type == 'seriemouseleave';
                var item = node.parent('.attachments-container').find('.legend-item[data-index=' + chart_.index + ']:visible');
                if(needOpacity)
                    item.removeClass('hovered');
                else
                    item.addClass('hovered');
                this.changeLineOpacity(item, needOpacity);
            },

            [ria.mvc.DomEventBind('click', '.gp-title-block')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function collapseClick(node, event){
                var nodeT = new ria.dom.Dom(event.target);
                var dom = this.dom;
                if(!nodeT.isOrInside('.no-loading')){
                    var parent = node.parent('.big-grading-period-container');

                    var gpData = parent.find('.gp-data');

                    if(parent.hasClass('open')){
                        jQuery(gpData.valueOf()).animate({
                            height: 0
                        }, 500);

                        gpData.addClass('with-data');

                        setTimeout(function(){
                            parent.removeClass('open');
                        }, 500);
                    }else{
                        var items = this.dom.find('.big-grading-period-container.open');
                        var itemsGp = items.find('.gp-data');
                        jQuery(itemsGp.valueOf()).animate({height: 0}, 500);
                        if(gpData.hasClass('with-data')){
                            gpData.removeClass('with-data');
                            this.openGradingPeriod(gpData);
                        }else{
                            parent.find('.load-grading-period').trigger('submit');
                        }
                        dom.find('.gp-data.with-data')
                            .setHTML('')
                            .removeClass('with-data');
                        setTimeout(function(){
                            items.removeClass('open');
                            itemsGp.setHTML('');
                        }, 500);
                    }
                }
            },

            [ria.mvc.DomEventBind('change', '.avg-select')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function selectChange(node, event, selected_){
                node.parent('form').trigger('submit');
            },

            [ria.mvc.DomEventBind('click', '.avg-item:not(.selected)')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function avgClick(node, event){
                node.parent('.big-grading-period-container').find('.avg-select').setValue(node.getData('id'));
            },

            function openGradingPeriod(container){
                container.parent('.big-grading-period-container').addClass('open');
                var annContainer = container.find('.people-list');
                container.setCss('height', 0);
                jQuery(container.valueOf()).animate({
                    height: (annContainer.height() + parseInt(annContainer.getCss('margin-bottom'), 10))
                }, 500, function(){
                    container.setCss('height', 'auto');
                });
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.grading.GradingPeriodFinalGradeTpl, 'load-gp')],
            VOID, function updateGradingPeriodPart1(tpl, model, msg_) {
                if(model.getCurrentAverage()){
                    var container = this.dom.find('.big-grading-period-container[data-grading-period-id=' + model.getGradingPeriod().getId().valueOf() + ']');
                    tpl.options({
                        classId: this.getClassId()
                    });
                    tpl.renderTo(container.setHTML(''));
                    setTimeout(function(){
                        this.openGradingPeriod(container.find('.gp-data'));
                    }.bind(this), 1);
                }
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.grading.FinalGradeStudentBlockTpl, chlk.activities.lib.DontShowLoader())],
            VOID, function updateStudentBlock(tpl, model, msg_) {
                var row = this.dom.find('.row[data-student-id=' + model.getStudent().getId().valueOf() + ']');
                tpl.options({
                    selected: row.hasClass('selected'),
                    index: parseInt(row.getAttr('index'), 10),
                    gradingPeriodId: new chlk.models.id.GradingPeriodId(row.parent('.big-grading-period-container').getData('grading-period-id'))
                });
                var html = new ria.dom.Dom(tpl.render());
                html.insertBefore(row);
                row.next('.attachments-container').remove();
                row.remove();
            },

            chlk.models.id.ClassId, 'classId',

            ArrayOf(chlk.models.grading.AvgComment), 'gradingComments',

            [ria.mvc.DomEventBind('keypress', '.grade-input')],
            [[ria.dom.Dom, ria.dom.Event]],
            function inputKeyPress(node, event){
                if(event.keyCode == ria.dom.Keys.ENTER){
                    if(!node.hasClass('error')){
                        var row = node.parent('.row');
                        if(!node.hasClass('error'))
                            this.selectRow(this.dom.find('.grades-individual').find('.row:eq(' + (parseInt(row.getAttr('index'),10) + 1) + ')'));
                        event.preventDefault();
                        return false;
                    }
                }
            },

            function selectRow(row){
                if(row.exists())
                    this.dom.find('.grades-individual').trigger(chlk.controls.GridEvents.SELECT_ROW.valueOf(), [row, parseInt(row.getAttr('index'), 10)]);
                else
                    this.dom.find('.row.selected').find('.grading-form').trigger('submit');
            },

            [ria.mvc.DomEventBind('chenge', '.grade-input')],
            [[ria.dom.Dom, ria.dom.Event]],
            function inputChange(node, event){
                node.parent('form').trigger('submit');
            },

            [ria.mvc.DomEventBind('contextmenu', '.grade-input')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function gradeMouseDown(node, event){
                node.parent().find('.grading-input-popup').show();
                return false;
            },

            [ria.mvc.DomEventBind('click', '.grading-input-popup')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function gradingPopUpClick(node, event){
                setTimeout(function(){
                    node.parent('form').find('.grade-input').trigger('focus');
                }, 1)
            },

            [ria.mvc.DomEventBind('change', '.exempt-checkbox')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function exemptChange(node, event, options_){
                node.parent('form').find('.grade-input').setValue('');
            },

            [ria.mvc.DomEventBind('submit', 'form.update-grade-form')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function submitForm(node, event){
                if(node.find('.grade-input').hasClass('error'))
                    return false;
                var row = node.parent('.row');
                var container = row.find('.top-content');
                if(container.hasClass('loading'))
                    return false;
                row.find('.grading-input-popup').hide();
                var input = node.find('.grade-input');
                var value = (input.getValue() || '').toLowerCase();
                if(value == 'exempt'){
                    input.setValue(input.getData('grade-value'));
                    node.find('.exempt-checkbox').setValue(true);
                }else{
                    node.find('.exempt-checkbox').setValue(false);
                }
                var checkbox = node.find('.exempt-checkbox');
                if((input.getValue() || '') == (input.getData('grade-value') || '') && (!checkbox.exists() || checkbox.getData('value') == checkbox.checked()))
                    return false;
                container.addClass('loading');
                return true;
            },

            [[String]],
            ArrayOf(String), function getSuggestedValues(text){
                var text = text.toLowerCase();
                var res = [];
                this.getAllScores().forEach(function(score){
                    if(score.toLowerCase().indexOf(text) == 0)
                        res.push(score);
                });
                return res;
            },

            [ria.mvc.DomEventBind('keyup', '.grade-input')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function gradeKeyUp(node, event){
                var suggestions = [];
                var isDown = event.keyCode == ria.dom.Keys.DOWN.valueOf();
                var isUp = event.keyCode == ria.dom.Keys.UP.valueOf();
                var list = this.dom.find('.autocomplete-list:visible');
                var value = (node.getValue() || '').trim();
                if(!value){
                    node.addClass('empty-grade');
                    node.removeClass('error');
                }
                else{
                    node.removeClass('empty-grade');
                }
                if(!isDown && !isUp){
                    if(event.keyCode == ria.dom.Keys.ENTER.valueOf()){
                        if(!node.hasClass('error')){
                            if(list.exists() && list.find('.see-all').hasClass('hovered'))
                                list.find('.see-all').trigger('click');
                        }
                        return false;
                    }else{
                        if(value){
                            var text = node.getValue() ? node.getValue().trim() : '';
                            var parsed = parseFloat(text);
                            if(!Number.isNaN(parsed)){
                                node.removeClass('error');
                                node.removeClass('not-equals');
                                if(text && parsed != text){
                                    node.addClass('error');
                                }else{
                                    this.hideDropDown();
                                }
                            }else{
                                suggestions = text  ? this.getSuggestedValues(text) : [];
                                if(!suggestions.length)
                                    node.addClass('error');
                                else{
                                    node.removeClass('error');
                                    var p = false;
                                    suggestions.forEach(function(item){
                                        if(item.toLowerCase() == node.getValue().toLowerCase())
                                            p = true;
                                    });
                                    if(p){
                                        node.removeClass('not-equals');
                                    }else{
                                        node.addClass('not-equals');
                                    }
                                }

                                this.updateDropDown(suggestions, node);
                            }
                        }
                    }
                    this.updateDropDown(suggestions, node);
                }
                return true;
            },

            [ria.mvc.DomEventBind('click', '.see-all')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function seeAllClick(node, event){
                var input = this.dom.find('.row.selected').find('.grade-input');
                input.removeClass('not-equals');
                this.updateDropDown(this.getAllScores(), input, true);
                return false;
            },

            VOID, function updateDropDown(suggestions, node, all_){
                var list = this.dom.find('.autocomplete-list');
                if(suggestions.length || node.hasClass('error')){
                    var html = '<div class="autocomplete-item">' + suggestions.join('</div><div class="autocomplete-item">') + '</div>';
                    if(!all_){
                        html += '<div class="autocomplete-item see-all">See all »</div>';
                        var top = node.offset().top - list.parent().offset().top + node.height() + 43;
                        var left = node.offset().left - list.parent().offset().left + 61;
                        list.setCss('top', top)
                            .setCss('left', left);
                    }
                    list.setHTML(html)
                        .show();
                }else{
                    this.hideDropDown();
                }
            },

            [ria.mvc.DomEventBind('mouseover', '.autocomplete-item')],
            [[ria.dom.Dom, ria.dom.Event]],
            function itemHover(node, event){
                if(!node.hasClass('hovered'))
                    node.parent().find('.hovered').removeClass('hovered');
                node.addClass('hovered');
            },

            [ria.mvc.DomEventBind('click', '.autocomplete-item:not(.see-all)')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function listItemBtnClick(node, event){
                var text = node.getHTML().trim();
                var value = text;
                var input = this.dom.find('.row.selected').find('.grade-input');
                input.removeClass('not-equals');
                input.setValue(text);
                input.removeClass('error');
                input.parent('form').trigger('submit');
                this.hideDropDown();
            },

            VOID, function hideDropDown(){
                var list = this.dom.find('.autocomplete-list');
                list.setHTML('')
                    .hide();
            },

            ArrayOf(String), 'allScores',

            OVERRIDE, VOID, function onRender_(model){
                BASE(model);
                this.setClassId(model.getTopData().getSelectedItemId());
                this.setGradingComments(model.getGradingComments());
                var allScores = [];
                model.getAlphaGrades().forEach(function(item){
                    allScores.push(item.getName());
                });
                this.setAllScores(allScores);
                this.openGradingPeriod(this.dom.find('.open.big-grading-period-container').find('.gp-data'));
                document.addEventListener('click', this.documentClickHandler, true);
            },

            OVERRIDE, VOID, function onStop_() {
                document.removeEventListener('click', this.documentClickHandler, true);
            },

            function documentClickHandler(event){
                var target = new ria.dom.Dom(event.target),
                    input = this.dom.find('.grade-input:visible');
                if(input.exists() && !target.isOrInside('.input-container.grade') && !target.isOrInside('.autocomplete-list') && !target.isOrInside('.action-link')){
                    this.dom.find('.grading-input-popup').hide();
                    input.parent('form').trigger('submit');
                }
            },

            [ria.mvc.DomEventBind(chlk.controls.GridEvents.SELECT_ROW.valueOf(), '.final-grid')],
            [[ria.dom.Dom, ria.dom.Event, ria.dom.Dom, Number]],
            function selectStudent(node, event, row, index){
                clearTimeout(slideTimeout);
                this.dom.find('.selected-value').setValue(index);
                slideTimeout = setTimeout(function(){
                    var target = node.find('.attachments-container:eq(' + index + ')');
                    if(!target.hasClass('opened')){
                        node.find('.attachments-container:eq(' + index + ')').slideDown(500);
                        row.find('.grade-triangle').addClass('down');
                    }
                }, 500);
            },

            [ria.mvc.DomEventBind(chlk.controls.GridEvents.DESELECT_ROW.valueOf(), '.final-grid')],
            [[ria.dom.Dom, ria.dom.Event, ria.dom.Dom, Number]],
            function deSelectStudent(node, event, row, index){
                node.find('.attachments-container:eq(' + index + ')').slideUp(250);
                row.find('.grade-triangle').removeClass('down');
                row.find('.grading-form').trigger('submit');
            }
        ]);
});