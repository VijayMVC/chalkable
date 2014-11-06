REQUIRE('chlk.templates.grading.GradingClassSummaryGridTpl');
REQUIRE('chlk.templates.grading.GradingInputTpl');
REQUIRE('chlk.templates.grading.ShortGradingClassSummaryGridItemsTpl');
REQUIRE('chlk.templates.grading.TeacherClassGradingGridSummaryCellTpl');
REQUIRE('chlk.templates.grading.ShortGradingClassSummaryGridAvgsTpl');
REQUIRE('chlk.templates.grading.GradingCommentsTpl');
REQUIRE('chlk.templates.grading.StudentAverageTpl');
REQUIRE('chlk.templates.grading.AvgCodesPopupTpl');
REQUIRE('chlk.templates.grading.StudentAverageInputTpl');

REQUIRE('chlk.activities.lib.TemplatePage');

REQUIRE('chlk.models.common.Array');
REQUIRE('chlk.models.announcement.ShortStudentAnnouncementViewData');
REQUIRE('chlk.models.grading.AvgCodesPopupViewData');

NAMESPACE('chlk.activities.grading', function () {

    var gradingGridTimer;

    function reorder(source, remap, studendIdProvider) {
        VALIDATE_ARGS(['source', 'remap', 'studendIdProvider'], [[Array], [Object], [Function]], [source, remap, studendIdProvider]);

        var result = new Array(source.length);
        source.forEach(function (_) {
            var studentId = studendIdProvider(_);

            VALIDATE_ARG('studentId', [chlk.models.id.SchoolPersonId], studentId);

            var index = remap[studentId];

            Assert(index != null);

            result[index] = _;
        });
        return result;
    }

    function strcmp(_1, _2) {
        var v1 = _1[1], v2 = _2[1];
        v1 = v1 != null ? v1 : Number.NEGATIVE_INFINITY;
        v2 = v2 != null ? v2 : Number.NEGATIVE_INFINITY;
        return v1 < v2 ? -1 : v1 > v2 ? 1 : _1[0] - _2[0];
    }

    /** @class chlk.activities.grading.GradingClassSummaryGridPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.grading.GradingClassSummaryGridTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.grading.GradingCommentsTpl, chlk.activities.lib.DontShowLoader(), '.grading-comments-list', ria.mvc.PartialUpdateRuleActions.Replace)],
        'GradingClassSummaryGridPage', EXTENDS(chlk.activities.lib.TemplatePage), [

            ArrayOf(chlk.models.grading.AvgComment), 'gradingComments',

            function $() {
                BASE();

                this._lastModel = null;
            },

            [ria.mvc.DomEventBind('click', '.mp-title')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function collapseClick(node, event){
                var nodeT = new ria.dom.Dom(event.target);
                var dom = this.dom;
                if(!nodeT.hasClass('gp-button')){
                    var parent = node.parent('.marking-period-container');

                    var mpData = parent.find('.mp-data');

                    if(parent.hasClass('open')){
                        jQuery(mpData.valueOf()).animate({
                            height: 0
                        }, 500);

                        mpData.addClass('with-data');

                        setTimeout(function(){
                            parent.removeClass('open');
                            //mpData.setHTML('');
                        }, 500);
                    }else{
                        var items = this.dom.find('.marking-period-container.open');
                        var itemsMp = items.find('.mp-data');
                        jQuery(itemsMp.valueOf()).animate({height: 0}, 500);
                        if(mpData.hasClass('with-data')){
                            mpData.removeClass('with-data');
                            this.openGradingPeriod(mpData);
                        }else{
                            clearTimeout(gradingGridTimer);
                            parent.find('.load-grading-period').trigger('submit');
                        }
                        dom.find('.mp-data.with-data')
                            .setHTML('')
                            .removeClass('with-data');
                        setTimeout(function(){
                            items.removeClass('open');
                            itemsMp.setHTML('');
                        }, 500);
                        //parent.addClass('open');

                    }
                }
            },

            [ria.mvc.DomEventBind('change', '.dropped-checkbox')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function droppedChange(node, event, options_){
                if(!node.checked()){
                    var input = node.parent('form').find('.grade-autocomplete');
                    input.setValue(input.getData('grade-value'));
                }
            },

            [ria.mvc.DomEventBind('change', '.codes-select')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function codesSelectChange(node, event, options_){
                var option = node.find(':selected');
                var code = option.getData('code');
                node.parent('.row').find('.code-input').setValue(code);
            },

            [ria.mvc.DomEventBind('change', '.code-input')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function codeChange(node, event){
                var select = node.parent('.row').find('.codes-select');
                var value = node.getValue() ? node.getValue().toUpperCase() : '';
                var option = select.find('[data-code=' + value + ']');
                if(option.exists()){
                    select.setValue(option.getAttr('value'));
                    select.trigger('liszt:updated');
                }else{
                    select.setValue('');
                    select.trigger('liszt:updated');
                }
            },

            [ria.mvc.DomEventBind('keyup', '.comment-value')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function commentKeyUp(node, event, options_){
                var popUp = node.parent().find('.grading-comments-list');
                if(popUp.is(':visible') && (event.which == ria.dom.Keys.UP.valueOf()
                    || event.which == ria.dom.Keys.DOWN.valueOf() || event.which == ria.dom.Keys.ENTER.valueOf())
                    && popUp.find('.item').exists()){
                        var selected = popUp.find('.item.selected'), next = selected;
                        if(!selected.exists())
                            selected = popUp.find('.item:first');
                        switch(event.which){
                            case ria.dom.Keys.UP.valueOf():
                                if(selected.previous().exists()){
                                    selected.removeClass('selected');
                                    selected.previous().addClass('selected');
                                }
                                break;
                            case ria.dom.Keys.DOWN.valueOf():
                                if(selected.next().exists()){
                                    selected.removeClass('selected');
                                    selected.next().addClass('selected');
                                }
                                break;
                            case ria.dom.Keys.ENTER.valueOf():
                                this.setCommentByNode(next);
                                break;
                        }
                }else{
                    if(node.getValue() && node.getValue().trim())
                        popUp.hide();
                    else
                        popUp.show();
                }
            },

            [ria.mvc.DomEventBind('change', '.exempt-checkbox')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function exemptChange(node, event, options_){
                var input = node.parent('form').find('.value-input');
                if(node.checked())
                    input.setValue('');
                else{
                    var oldValue = input.getData('grade-value');
                    input.setValue(oldValue.toLowerCase() == 'exempt' ? '' : oldValue);
                }

            },

            function getScores(node){
                var parent = node.parent('.grade-value'), scores;
                if(parent.hasClass('avg-value-container')){
                    scores = this.getStandardScores();
                    if(parent.find('.grade-info').getData('may-be-exempt'))
                        scores = scores.concat(['Exempt']);
                }else{
                    scores = this.getAllScores();
                    if(parent.getData('able-drop-student-score'))
                        scores = scores.concat(['Dropped', 'Dropped (fill all)']);
                    if(parent.getData('able-exempt-student-score'))
                        scores = scores.concat(['Exempt', 'Exempt (fill all)']);
                }

                return scores;
            },

            function openGradingPeriod(container){
                container.parent('.marking-period-container').addClass('open');
                var annContainer = container.find('.ann-types-container');
                container.setCss('height', 0);
                jQuery(container.valueOf()).animate({
                    height: (annContainer.height() + parseInt(annContainer.getCss('margin-bottom'), 10))
                }, 500);
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.grading.ShortGradingClassSummaryGridItemsTpl)],
            VOID, function updateGradingPeriodPart(tpl, model, msg_) {
                var container = this.dom.find('.mp-data[data-grading-period-id=' + model.getGradingPeriod().getId().valueOf() + ']');
                var tooltipText = model.getTooltipText(), parent = container.parent();
                tpl.options({
                    classId: this.getClassId()
                });
                tpl.renderTo(container.setHTML(''));
                if(model.getGradingItems().length && model.getStudents().length){
                    parent.removeClass('no-items');
                }else{
                    parent.addClass('no-items');
                }
                setTimeout(function(){
                    this.openGradingPeriod(container);
                    parent.find('.mp-name').setData('tooltip', tooltipText);
                }.bind(this), 1);

            },

            function calculateGradesAvg(gradingitem, round_){
                var studentAnnouncements = gradingitem.studentannouncements;
                if (!studentAnnouncements)
                    return null;

                var gradedStudentCount = 0, sum = 0, numericGrade, gradeValue;
                var items = studentAnnouncements.items || [], classAvg = null;
                items.forEach(function(item){
                    numericGrade = item.numericgradevalue;
                    gradeValue = item.gradevalue;
                    if(!item.dropped
                        && !item.isincomplete
                        && (gradeValue && gradeValue.toLowerCase() != 'ps'
                            && gradeValue.toLowerCase() != 'wd'
                            && gradeValue.toLowerCase() != 'nc')
                        && item.includeinaverage
                        && (numericGrade || numericGrade == 0 || item.gradevalue == 0 || item.gradevalue)){
                            gradedStudentCount++;
                            sum += (numericGrade || 0);
                    }
                });
                studentAnnouncements.gradedStudentCount = gradedStudentCount;
                if(gradedStudentCount){
                    classAvg = (sum / gradedStudentCount).toFixed(round_ ? 0 : 2)
                }
                studentAnnouncements.classAvg = classAvg;
                return classAvg;
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.grading.ShortGradingClassSummaryGridAvgsTpl)],
            VOID, function updateGradingPeriodAvgs(tpl, model, msg_) {
                var value = model.getValue(), calculateGradesAvg, that = this;
                var container = this.dom.find('.mp-data[data-grading-period-id=' + value.gradingperiod.id + ']');
                var tooltipText = (value.avg != null ? Msg.Avg + " " + value.avg : 'No grades yet');
                var dom = this.dom;
                if(!value.isAvg){
                    var rowIndex = parseInt(dom.find('.active-row').getAttr('row-index'), 10);
                    model.getValue().rowIndex = rowIndex;
                    var avgs = container.find('.avgs-container');
                    var html = new ria.dom.Dom().fromHTML(tpl.render());
                    html.prependTo(avgs.parent());
                    avgs.remove();
                }else{
                    value.totalavarages.forEach(function(item){
                        var grade = item.totalaverage;
                        var value = grade || grade == 0 ? grade.toFixed(value && value.rounddisplayedaverages ? 0 :2) : '';
                        dom.find('.total-average[data-average-id=' + item.averageid + ']').setHTML(value.toString());
                    });
                }

                value.totalpoints && value.totalpoints.forEach(function(item, index){
                    var value = item.maxtotalpoint ? (item.totalpoint + '/' + item.maxtotalpoint) : '';
                    dom.find('.total-point[row-index=' + index + ']').setHTML(value);
                });

                value.gradingitems.forEach(function(item){
                    calculateGradesAvg = that.calculateGradesAvg(item, value.rounddisplayedaverages);
                    dom.find('.avg-' + item.id).setHTML(calculateGradesAvg || calculateGradesAvg === 0 ? calculateGradesAvg.toString() : '');
                });
                container.parent().find('.mp-name').setData('tooltip', tooltipText);

            },

            [ria.mvc.PartialUpdateRule(chlk.templates.grading.TeacherClassGradingGridSummaryCellTpl, chlk.activities.lib.DontShowLoader())],
            VOID, function updateGradingCell(tpl, model, msg_) {
                var container = this.dom.find('.item-' + model.getAnnouncementId().valueOf() + '-' + model.getStudentId());
                tpl.options({
                    maxScore: container.getData('max-score')
                });
                tpl.renderTo(container.setHTML(''));
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.grading.StudentAverageTpl, chlk.activities.lib.DontShowLoader())],
            VOID, function updateAvgCell(tpl, model, msg_) {
                var container = this.dom.find('.avg-value-container[data-student-id=' + model.getStudentId().valueOf()
                    + '][data-average-id=' + model.getAverageId().valueOf() + ']');
                if(!model.getGradingPeriodId()){
                    var input = container.find('.value-input');
                    //input.setValue(input.getData('grade-value'));
                    var textNode = container.find('.grade-text');
                    textNode.setHTML(textNode.getData('value'));
                    container.find('.for-edit').removeClass('for-edit');
                    container.find('.grading-form').remove();
                }else{
                    tpl.options({
                        gradingPeriodId: new chlk.models.id.GradingPeriodId(container.getData('grading-period-id')),
                        ableDisplayAlphaGrades: !!container.getData('able-display-alpha-grades'),
                        roundDisplayedAverages: !!container.getData('able-round-displayed-averages')
                    });
                    tpl.renderTo(container.setHTML(''));
                }
            },

            ArrayOf(String), 'allScores',

            ArrayOf(String), 'standardScores',

            [[String, ria.dom.Dom]],
            ArrayOf(String), function getSuggestedValues(text, node){
                var text = text.toLowerCase();
                var res = [];
                this.getScores(node).forEach(function(score){
                    if(score.toLowerCase().indexOf(text) == 0)
                        res.push(score);
                });
                return res;
            },

            VOID, function updateDropDown(suggestions, node, all_){
                var list = this.dom.find('.autocomplete-list');
                if(suggestions.length || node.hasClass('error')){
                    var html = '';
                    suggestions.forEach(function(item){
                        html += '<div class="autocomplete-item" data-tooltip-type="overflow" data-tooltip="' + item + '">' + item + '</div>';
                    });
                    if(!all_)
                        html += '<div class="autocomplete-item see-all">See all »</div>';
                    var top = node.offset().top - list.parent().offset().top + node.height() + 43;
                    var left = node.offset().left - list.parent().offset().left + 61;
                    list.setCss('top', top)
                        .setCss('left', left)
                        .setHTML(html)
                        .show();
                }else{
                    this.hideDropDown();
                }
            },

            VOID, function hideDropDown(){
                var list = this.dom.find('.autocomplete-list');
                list.setHTML('')
                    .hide();
            },

            [ria.mvc.DomEventBind('change', '.grading-select')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            function gradingSelectChange(node, event, selected_){
                clearTimeout(gradingGridTimer);
                if(node.getValue())
                    node.addClass('with-value');
                else
                    node.removeClass('with-value');
                var form = node.parent('form');
                var hidden = form.find('.not-calculate-grid');
                hidden.setValue(true);
                form.trigger('submit');
                setTimeout(function(){
                    hidden.setValue(false);
                }, 1)
            },

            [ria.mvc.DomEventBind('contextmenu', '.value-input')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function gradeMouseDown(node, event){
                var cell = node.parent('.grade-value');
                if(!cell.hasClass('avg-value-container') || cell.find('.grade-info').getData('may-be-exempt')){
                    node.parent().find('.grading-input-popup').show();
                    return false;
                }
                return true;
            },

            [ria.mvc.DomEventBind('focus', '.value-input')],
            [[ria.dom.Dom, ria.dom.Event]],
            function gradeFocus(node, event){
                node.select();
            },

            function submitActiveForm(){
                var activeCell = this.dom.find('.active-cell');
                if(activeCell.exists()){
                    activeCell.find('form').trigger('submit');
                }
                activeCell.removeClass('active-cell');
                activeCell.find('.grading-form').remove();
            },

            [ria.mvc.DomEventBind('contextmenu', '.edit-cell, .avg-text')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function cellContextMenu(node, event){
                var cell = node.parent('.grade-value');
                if(cell.hasClass('gradable')){
                    this.submitActiveForm();
                    this.showCell(cell);
                    setTimeout(function(){
                        cell.find('.value-input').trigger('contextmenu');
                    }, 1);
                    return false;
                }
                return true;
            },

            [ria.mvc.DomEventBind('click', '.value-input, .grading-input-popup')],
            [[ria.dom.Dom, ria.dom.Event]],
            function gradeClick(node, event){
                this.hideDropDown();
            },

            [ria.mvc.DomEventBind('dblclick', '.value-input')],
            [[ria.dom.Dom, ria.dom.Event]],
            function inputDblClickClick(node, event){
                node.removeClass('not-equals');
                this.updateDropDown(this.getScores(node), node, true);
            },

            OVERRIDE, VOID, function onStop_() {
                BASE();
                new ria.dom.Dom().off('click.grading_popup');
                new ria.dom.Dom().off('click.grade');
            },

            [ria.mvc.DomEventBind('keyup', '.value-input')],
            [[ria.dom.Dom, ria.dom.Event]],
            function gradeKeyUp(node, event){
                var suggestions = [], cell = node.parent('.active-cell');
                var list = this.dom.find('.autocomplete-list:visible');
                var value = (node.getValue() || '').trim(), fillItem = node.parent().find('.fill-grade');
                if(!value){
                    node.addClass('empty-grade');
                    node.removeClass('error');
                }
                else{
                    node.removeClass('empty-grade');
                }
                switch(value.toLowerCase()){
                    case Msg.Dropped.toLowerCase():
                    case Msg.Exempt.toLowerCase():
                    case Msg.Incomplete.toLowerCase():
                    case Msg.Late.toLowerCase(): fillItem.setAttr('disabled', true);break;
                    default: value ? fillItem.setAttr('disabled', false) : fillItem.setAttr('disabled', true);
                }
                if(!node.hasClass('cell-showed') &&
                    event.keyCode != ria.dom.Keys.UP.valueOf() &&
                    event.keyCode != ria.dom.Keys.DOWN.valueOf()
                ){
                    if(event.keyCode == ria.dom.Keys.ENTER.valueOf() && !node.hasClass('error')){

                    }else{
                        node.removeClass('not-equals');
                        if(value){
                            var text = node.getValue() ? node.getValue().trim() : '';
                            var parsed = parseFloat(text);
                            if(parsed || parsed == 0){
                                node.removeClass('error');
                                if(parsed != text || parsed > 9999.99 || parsed < -9999.99){
                                    node.addClass('error');
                                }else{
                                    this.hideDropDown();
                                }
                            }else{
                                suggestions = text  ? this.getSuggestedValues(text, node) : [];
                                if(!suggestions.length)
                                    node.addClass('error');
                                else{
                                    node.removeClass('error');
                                    var p = false;
                                    suggestions.forEach(function(item){
                                        if(item.toLowerCase() == node.getValue().toLowerCase())
                                            p = true;
                                    });
                                    if(!p){
                                        node.addClass('not-equals');
                                    }
                                }
                            }
                        }
                    }
                    if(!(event.keyCode == ria.dom.Keys.ENTER.valueOf() && list.exists()))
                        this.updateDropDown(suggestions, node);
                }
                node.removeClass('cell-showed');
                setTimeout(function(cell){
                    var node = cell.find('.grade-autocomplete');
                    var numericValue = parseFloat(node.getValue());
                    if(numericValue == node.getValue()){
                        var model = this.getModelFromCell(cell);
                        this.updateFlagByModel(model, cell);
                    }
                }.bind(this, cell), 10);

            },

            [ria.mvc.DomEventBind('keydown', '.value-input')],
            [[ria.dom.Dom, ria.dom.Event]],
            function gradeUpDownKeyDown(node, event){
                var list = this.dom.find('.autocomplete-list:visible'), canGoDown, hovered;

                if(event.keyCode == ria.dom.Keys.ENTER.valueOf()){
                    if(list.exists()){
                        hovered = list.find('.hovered');
                        if(hovered.exists()){
                            hovered.trigger('click');

                            return false;
                        }
                    }
                    if(node.hasClass('error'))
                        return false;

                    this.setItemValue(node.getValue(), node, true);

                }
                var isDown = event.keyCode == ria.dom.Keys.DOWN.valueOf();
                var isUp = event.keyCode == ria.dom.Keys.UP.valueOf();
                if(isDown || isUp){
                    if(list.exists()){
                        hovered = list.find('.hovered');
                        if(hovered.exists()){
                            if(isDown){
                                if(hovered.next().exists()){
                                    hovered.removeClass('hovered');
                                    hovered.next().addClass('hovered');
                                }else
                                    canGoDown = true;
                            }
                            if(isUp && hovered.previous().exists()){
                                hovered.removeClass('hovered');
                                hovered.previous().addClass('hovered');
                            }
                        }else{
                            if(isDown){
                                list.find('.autocomplete-item:eq(0)').addClass('hovered');
                            }
                        }
                        event.stopPropagation();
                    }
                }

                var curCell = node.parent('.grade-value'), cell, curBlock, valueInput, needsTimeout;
                switch(event.keyCode){
                    case ria.dom.Keys.UP.valueOf():
                        if(!list.is(':visible') || !list.find('.hovered').previous().exists())
                            cell = curCell.previous('.grade-value');
                        break;
                    case ria.dom.Keys.DOWN.valueOf():
                        if(!list.is(':visible') || canGoDown)
                            cell = curCell.next('.grade-value');
                        break;
                }

                switch(event.keyCode){
                    case ria.dom.Keys.LEFT.valueOf():
                        valueInput = curCell.find('.value-input');
                        if(valueInput.getSelectedText() || valueInput.getCursorPosition() == 0){
                            curBlock = curCell.parent('.grade-container').previous('.grade-container');
                            if(curBlock.exists() && curBlock.hasClass('total-points'))
                                curBlock = curBlock.previous('.grade-container');
                            var nameContainer = curBlock.parent('.ann-types-container').find('.name-container');
                            if(curBlock.exists()){
                                if(curBlock.offset().left < (nameContainer.offset().left + nameContainer.width())){
                                    curBlock.parent('.grid-toolbar').find('.prev-button').trigger('click');
                                    needsTimeout = true;
                                }
                                cell = curBlock.find('.grade-value[row-index=' + curCell.getAttr('row-index') + ']');
                            }
                        }
                        break;
                    case ria.dom.Keys.RIGHT.valueOf():
                        valueInput = curCell.find('.value-input');
                        var value = valueInput.getValue() || '';
                        if(valueInput.getSelectedText() || valueInput.getCursorPosition() == value.length){
                            curBlock = curCell.parent('.grade-container').next('.grade-container');
                            if(curBlock.exists() && curBlock.hasClass('total-points'))
                                curBlock = curBlock.next('.grade-container');
                            if(curBlock.exists()){
                                if(curBlock.hasClass('transparent-container')){
                                    curBlock.parent('.grid-toolbar').find('.next-button').trigger('click');
                                    needsTimeout = true;
                                }
                                cell = curBlock.find('.grade-value[row-index=' + curCell.getAttr('row-index') + ']');
                            }
                        }
                        break;
                }
                if(cell && cell.exists() && cell.hasClass('gradable')){
                    this.submitActiveForm();
                    if(needsTimeout)
                        setTimeout(function(){this.showCell(cell)}.bind(this), 500);
                    else{
                        this.showCell(cell);
                        cell.find('.value-input').addClass('cell-showed');
                    }
                }
            },

            [[chlk.models.announcement.ShortStudentAnnouncementViewData, ria.dom.Dom]],
            function updateFlagByModel(model, cell){
                var value = cell.find('.grade-autocomplete').getValue();
                model.setGradeValue(value);
                var flag = cell.find('.alert-flag');
                flag.removeClass(Msg.Error.toLowerCase())
                    .removeClass(Msg.Late.toLowerCase())
                    .removeClass(Msg.Incomplete.toLowerCase())
                    .removeClass(Msg.Absent.toLowerCase())
                    .removeClass(Msg.Multiple.toLowerCase());
                var maxValue = cell.getData('max-score');
                flag.addClass(model.getAlertClass(maxValue));
                flag.setData('tooltip', model.getTooltipText(maxValue));
            },

            [ria.mvc.DomEventBind('click', '.see-all')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function seeAllClick(node, event){
                var cell = this.dom.find('.active-cell');
                var input = cell.find('.value-input');
                input.removeClass('not-equals');
                this.updateDropDown(this.getScores(input), input, true);
                return false;
            },

            [ria.mvc.DomEventBind('click', '.grading-select + .chzn-container')],
            [[ria.dom.Dom, ria.dom.Event]],
            function clickSelect(node, event){
                event.stopPropagation();
            },

            [ria.mvc.DomEventBind('mouseover', '.autocomplete-item')],
            [[ria.dom.Dom, ria.dom.Event]],
            function itemHover(node, event){
                if(!node.hasClass('hovered'))
                    node.parent().find('.hovered').removeClass('hovered');
                node.addClass('hovered');
            },

            [ria.mvc.DomEventBind(chlk.controls.LRToolbarEvents.AFTER_RENDER.valueOf(), '.grid-toolbar')],
            [[ria.dom.Dom, ria.dom.Event]],
            function afterTbRender(node, event){
                this.beforeTbAnimation(node);
            },

            [ria.mvc.DomEventBind(chlk.controls.LRToolbarEvents.BEFORE_ANIMATION.valueOf(), '.grid-toolbar')],
            [[ria.dom.Dom, ria.dom.Event, Boolean, Number]],
            function beforeTbAnimation(toolbar, event_, isLeft_, index_){
                this.dom.find('.transparent-container').removeClass('transparent-container').removeClass('delay');
                var startIndex = index_ ? index_ * 5 + 5 : 5;
                var node = toolbar.find('.dotted-container:eq(' + startIndex + ')');
                if(!node.is(':last-child')){
                    if(isLeft_)
                        node.addClass('delay');
                    setTimeout(function(){
                        node.addClass('transparent-container');
                    },1);
                }
            },

            [ria.mvc.DomEventBind('click', '.comment-button')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function commentBtnClick(node, event){
                var active = this.dom.find('.active-cell');
                //if(active.find('.value-input').getValue()){
                    var popUp = this.dom.find('.chlk-pop-up-container.comment');
                    var main = this.dom.parent('#main');
                    var bottom = main.height() + main.offset().top - active.offset().top + 73;
                    var left = active.offset().left - main.offset().left - 54;
                    popUp.setCss('bottom', bottom);
                    popUp.setCss('left', left);
                    var comment = active.find('.comment-value').getValue();
                    popUp.find('textarea').setValue(comment);
                    popUp.show();
                    popUp.find('.grading-comments-list').show();
                    if(comment)
                        popUp.find('.grading-comments-list').hide();
                    setTimeout(function(){
                        popUp.find('.comment-value').trigger('focus');
                    }, 1);
                //}
            },

            [ria.mvc.DomEventBind('click', '.codes-button')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function codesBtnClick(node, event){
                var js = new ria.serialize.JsonSerializer();
                var active = this.dom.find('.active-cell');
                var text = active.find('.codes-text').getValue();
                if(text && JSON.parse(text)){
                    var res = JSON.parse(text);
                    var headers = js.deserialize(res, ArrayOf(chlk.models.grading.AvgCodeHeaderViewData));
                    var model = new chlk.models.grading.AvgCodesPopupViewData(headers, this.getGradingComments(), active.getData('student-name'),
                        active.getData('grading-period-name'), active.getData('average-id'), parseInt(active.getAttr('row-index'), 10));
                    var tpl = new chlk.templates.grading.AvgCodesPopupTpl();
                    tpl.assign(model);
                    var popUp = this.dom.find('.chlk-pop-up-container.codes');
                    popUp.find('.codes-content').setHTML(tpl.render());

                    var main = this.dom.parent('#main');
                    var bottom = main.height() + main.offset().top - active.offset().top + 73;
                    var left = active.offset().left - main.offset().left - 260;
                    popUp.setCss('bottom', bottom);
                    popUp.setCss('left', left);
                    popUp.show();
                }
            },

            function setCommentByNode(node){
                var popUp = node.parent('.chlk-pop-up-container');
                var input = popUp.find('.comment-value');
                input.setValue(node.getHTML());
                popUp.find('.grading-comments-list').hide();
            },

            [ria.mvc.DomEventBind('click', '.grading-comments-list .item')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function commentItemClick(node, event){
                this.setCommentByNode(node);
            },

            [ria.mvc.DomEventBind('mouseover', '.grading-comments-list .item')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function commentItemMouseOver(node, event){
                if(!node.hasClass('selected')){
                    node.parent('.grading-comments-list').find('.selected').removeClass('selected');
                    node.addClass('selected');
                }
            },

            [ria.mvc.DomEventBind('click', '.cancel-comment')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function cancelBtnClick(node, event){
                node.parent('.chlk-pop-up-container.comment').hide();
                var cell = this.dom.find('.active-cell');
                setTimeout(function(){
                    cell.find('input').trigger('focus');
                }, 1);
                var commentInput = cell.find('.comment-value');
                commentInput.setValue(commentInput.getData('comment'));
            },

            [ria.mvc.DomEventBind('click', '.cancel-codes')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function cancelCodesBtnClick(node, event){
                node.parent('.chlk-pop-up-container.codes').hide();
            },

            [ria.mvc.DomEventBind('click', '.save-codes')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function saveCodesBtnClick(node, event){
                var popUp = node.parent('.chlk-pop-up-container.codes');
                var res = [], o;
                popUp.find('.row:not(.header)').forEach(function(item){
                    var input = item.find('.code-input');
                    var select = item.find('.codes-select');
                    o = {
                        headerId: input.getData('header-id'),
                        headerName: input.getData('header-name'),
                        gradingComment: input.getValue() ? {
                            code: input.getValue(),
                            id: parseInt(select.getValue(), 10),
                            comment: select.find('option:selected').getHTML()
                        }: null
                    };
                    res.push(o);
                });
                var averageId = node.getData('average-id');
                var rowIndex = node.getData('row-index');
                var cell = this.dom.find('.avg-value-container[row-index=' + rowIndex + '][data-average-id=' + averageId + ']');
                cell.find('input[name=codesString]').setValue(JSON.stringify(res));
                cell.find('.is-comment').setValue(true);
                var input = cell.find('.value-input');
                if(input.hasClass('error'))
                    input.setValue('input').getData('grade-value');
                cell.find('form').trigger('submit');
                popUp.hide();
            },

            [ria.mvc.DomEventBind('click', '.add-comment')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function addCommentBtnClick(node, event){
                var cell = this.dom.find('.active-cell');
                var commentInput = cell.find('.comment-value');
                var comment = node.parent('.chlk-pop-up-container').find('textarea').getValue();
                commentInput.setValue(comment).setData('comment', comment);
                node.parent('.chlk-pop-up-container.comment').hide();
                setTimeout(function(){
                    cell.find('input').trigger('focus');
                }, 1);
                this.updateValue(false);
            },

            [ria.mvc.DomEventBind('click', '.grading-input-popup')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function gradingPopUpClick(node, event){
                setTimeout(function(){
                    node.parent('form').find('.value-input').trigger('focus');
                }, 1)
            },

            [ria.mvc.DomEventBind('click', '.autocomplete-item:not(.see-all)')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function listItemBtnClick(node, event){
                var text = node.getHTML().trim();
                this.hideDropDown();
                var value = text, isFill = false;
                var cell = this.dom.find('.active-cell');
                cell.find('.error').removeClass('error');
                var input = cell.find('.value-input');
                if(text.toLowerCase().indexOf('fill') > -1){
                    isFill = true;
                    value = text.split('(fill all)')[0].trim();
                }
                input.removeClass('not-equals');
                this.setItemValue(value, input, true);
                if(isFill){
                    this.fillAllOneValue(cell, value);
                }
            },

            [[ria.dom.Dom, String, Boolean]],
            function setItemState_(node, stateName, selectNext_){
                node.setValue(node.getData('grade-value'));
                node.parent('form').find('[name=' + stateName +']').setValue(true);
                this.updateValue(selectNext_);
            },

            function setItemValue(value, input, selectNext){
                value = value || '';
                switch(value.toLowerCase()){
                    case Msg.Dropped.toLowerCase(): this.setItemState_(input, 'dropped', selectNext); break;
                    case Msg.Incomplete.toLowerCase(): this.setItemState_(input, 'isincomplete', selectNext); break;
                    case Msg.Late.toLowerCase(): this.setItemState_(input, 'islate', selectNext); break;
                    case Msg.Exempt.toLowerCase(): this.setItemState_(input, 'isexempt', selectNext); break;
                    default:{
                        var numericValue = parseFloat(value);
                        if(isNaN(numericValue) && value){
                            var allScores = this.getScores(input);
                            allScores = allScores.filter(function(score){
                                return score.toLowerCase() == value.toLowerCase();
                            });
                            if(allScores.length == 0) return;
                        }
                        input.setValue(value);
                        this.updateValue(selectNext);
                    }
                }
            },

            [[Boolean]],
            function updateValue(selectNext_){
                var activeCell = this.dom.find('.active-cell');
                activeCell.find('form').trigger('submit');

                if(selectNext_ && !activeCell.hasClass('avg-value-container')){
                    var nextCell = activeCell.next().find('.edit-cell');
                    setTimeout(function(){
                        if(nextCell.exists())
                            nextCell.trigger('click');
                        else
                            this.dom.trigger('click');
                    }.bind(this), 1);
                }
            },

            OVERRIDE, VOID, function onModelWait_(msg_) {
                BASE(msg_);
                if(msg_ == chlk.activities.lib.DontShowLoader()){
                    this.dom.find('.for-edit').setHTML('...');
                }
            },

            [ria.mvc.DomEventBind('submit', '.grading-form')],
            [[ria.dom.Dom, ria.dom.Event]],
            function gradingFormSubmit(node, event){
                var input = node.find('.value-input');
                if(!input.hasClass('error') && !input.hasClass('not-equals')){
                    node.find('.grading-input-popup').hide();
                    var value = (input.getValue() || '').toLowerCase();
                    var isAvg = node.hasClass('avg-form');
                    if(value == 'dropped' || value == 'exempt'){
                        input.setValue(input.getData('grade-value'));
                    }

                    var activeCell = node.parent('.grade-value');
                    this.dom.find('.autocomplete-list:visible').hide();
                    var model = this.getModelFromCell(activeCell), p = false;
                    var resModel = this.serializeFromForm(node);

                    if(isAvg){
                        var isComment = node.find('.is-comment').getValue();
                        var equalsValues = (input.getValue() || '') == (input.getData('grade-value').toString() || '');
                        if(model.isExempt() && resModel.isExempt() && value != 'exempt' )
                            node.find('[name=isexempt]').setValue(false);
                        p = equalsValues && !isComment && this.getBooleanValue(model.isExempt()) == this.getBooleanValue(resModel.isExempt());
                        if(!p && isComment && equalsValues && !node.parent('.grade-value').find('.edited').exists()){
                            input.setValue('');
                        }
                    }else{
                        p = (model.getGradeValue() || '') == (resModel.getGradeValue() || '') &&
                            this.getBooleanValue(model.isDropped()) == this.getBooleanValue(resModel.isDropped()) &&
                            this.getBooleanValue(model.isLate()) == this.getBooleanValue(resModel.isLate()) &&
                            this.getBooleanValue(model.isExempt()) == this.getBooleanValue(resModel.isExempt()) &&
                            this.getBooleanValue(model.isIncomplete()) == this.getBooleanValue(resModel.isIncomplete()) &&
                            (model.getComment() || "") == (resModel.getComment() || "");
                    }
                    if(p){
                        event.preventDefault();
                        return false;
                    }

                    if(!node.getData('able-drop')){
                        node.find('.dropped-checkbox').setValue(false);
                        node.find('.dropped-hidden').setValue(false);
                    }

                    var form = activeCell.parent('.marking-period-container').find('.load-grading-period');
                    if(isAvg){
                        activeCell.removeClass('active-cell');
                        activeCell.find('.grade-text').addClass('for-edit');
                    }else{
                        this.updateFlagByModel(model, activeCell);
                        activeCell.find('.grade-text').setHTML('...');
                    }
                    clearTimeout(gradingGridTimer);
                    this.addTimeOut(form, isAvg);
                    var mp = node.parent('.marking-period-container');
                    mp.find('.comment-button').hide();
                }else{
                    event.preventDefault();
                    return false;
                }
            },

            function addTimeOut(form, isAvg_){
                gradingGridTimer = setTimeout(function(){
                    form.find('.auto-update').setValue(true);
                    form.find('.avg-value').setValue(isAvg_);
                    form.trigger('submit');
                    setTimeout(function(){
                        form.find('.auto-update').setValue(false);
                        form.find('.avg-value').setValue(false);
                    }, 1);
                }, 5000);
            },

            chlk.models.id.ClassId, 'classId',

            OVERRIDE, VOID, function onRender_(model){
                BASE(model);
                var allScores = [], standardScores = [];
                this.setGradingComments(model.getGradingComments());
                model.getAlternateScores().forEach(function(item){
                    allScores.push(item.getName());
                    allScores.push(item.getName() + ' (fill all)');
                });
                model.getAlphaGrades().forEach(function(item){
                    allScores.push(item.getName());
                    standardScores.push(item.getName());
                    allScores.push(item.getName() + ' (fill all)');
                });

                this.setClassId(model.getTopData().getSelectedItemId());

                this.openGradingPeriod(this.dom.find('.open.marking-period-container').find('.mp-data'));

                allScores = allScores.concat(['Incomplete', 'Incomplete (fill all)', 'Late', 'Late (fill all)']);
                this.setAllScores(allScores);
                this.setStandardScores(standardScores);
                var dom = this.dom;
                var that = this;
                new ria.dom.Dom().on('click.grade', function(doc, event){
                    var node = new ria.dom.Dom(event.target);
                    if(!node.isOrInside('.grading-comments-list')){
                        dom.find('.grading-comments-list').hide();
                    }

                    var popUp = dom.find('.chlk-pop-up-container.comment, .chlk-pop-up-container.codes');
                    if(!node.hasClass('value-input') && !node.hasClass('arrow')
                        && !node.isOrInside('.chlk-pop-up-container.comment') && !node.isOrInside('.grading-input-popup')
                        && !node.isOrInside('.autocomplete-list') && !node.isOrInside('.chlk-pop-up-container.codes')){
                            dom.find('.autocomplete-list').setHTML('').hide();
                            if(!node.hasClass('gp-button')){
                                popUp.hide();
                                var parent = (node.hasClass('grade-value') && node.hasClass('gradable')) ? node : node.parent('.grade-value.gradable');

                                that.submitActiveForm();
                                if(parent.exists() && !node.hasClass('alert-flag')){
                                    that.showCell(parent);
                                }else{
                                    dom.find('.active-row').removeClass('active-row');
                                    dom.find('.gp-button').hide();
                                }
                            }
                    }
                });

                var dom = this.dom;
                new ria.dom.Dom().on('click.grading_popup', function(doc, event){
                    var node = new ria.dom.Dom(event.target);
                    if(!node.isOrInside('.grading-input-popup')){
                        dom.find('.grading-input-popup').hide();
                    }
                });
            },

            function showCell(parent){
                if(!parent.hasClass('active-row')){
                    var index = parent.getAttr('row-index');
                    this.dom.find('.active-row').removeClass('active-row');
                    this.dom.find('[row-index=' + index + ']').addClass('active-row');
                }
                parent.addClass('active-cell');

                var mp = parent.parent('.marking-period-container');
                this.addFormToActiveCell(parent);
                if(!parent.hasClass('avg-value-container')){
                    mp.find('.comment-button').show();
                    mp.find('.codes-button').hide();
                }else{
                    var value = parent.find('.grade-info').getData('codes-string');
                    if(value && value.length)
                        mp.find('.codes-button').show();
                    mp.find('.comment-button').hide();
                }

                setTimeout(function(){
                    parent.find('.value-input').trigger('focus');
                },1)
            },

            [[ria.dom.Dom]],
            Object, function getModelFromCell(cell){
                var node = cell.find('.grade-info'), model;
                if(cell.hasClass('avg-value-container'))
                    model = new chlk.models.grading.ShortStudentAverageInfo(
                        cell.getData('average-id'),
                        new chlk.models.id.GradingPeriodId(cell.getData('grading-period-id')),
                        parseFloat(node.getData('calculated-avg')),
                        parseFloat(node.getData('entered-avg')),
                        node.getData('calculated-alpha-grade'),
                        node.getData('entered-alpha-grade'),
                        this.getBooleanValue(node.getData('may-be-exempt')),
                        this.getBooleanValue(node.getData('exempt')),
                        new chlk.models.id.SchoolPersonId(node.getData('student-id')),
                        JSON.stringify(node.getData('codes-string'))
                    );
                else{
                    var grade = cell.find('.grade-text').getData('grade-value');
                    grade = grade ? grade.toString() : '';
                    model = new chlk.models.announcement.ShortStudentAnnouncementViewData(
                        new chlk.models.id.StudentAnnouncementId(node.getData('id')),
                        new chlk.models.id.AnnouncementId(node.getData('announcementid')),
                        new chlk.models.id.SchoolPersonId(node.getData('studentid')),
                        this.getBooleanValue(node.getData('dropped')),
                        this.getBooleanValue(node.getData('islate')),
                        this.getBooleanValue(node.getData('isexempt')),
                        this.getBooleanValue(node.getData('isabsent')),
                        this.getBooleanValue(node.getData('isincomplete')),
                        node.getData('comment'),
                        grade,
                        this.getBooleanValue(node.getData('isincludeinaverage'))
                    );
                }

                return model;
            },

            Boolean, function getBooleanValue(value){
                return value == 'false' ? false : !!value;
            },

            function addFormToActiveCell(cell, model_){
                var model, tpl;
                if(model_){
                    model = model_;
                    var studentId = cell.find('.grade-info').getData('studentid');
                    model.setStudentId(new chlk.models.id.SchoolPersonId(studentId));
                }else{
                    model = this.getModelFromCell(cell);
                }
                if(cell.hasClass('avg-value-container')){
                    tpl = new chlk.templates.grading.StudentAverageInputTpl();
                }else{
                    tpl = new chlk.templates.grading.GradingInputTpl();
                }

                tpl.assign(model);

                if(!cell.hasClass('avg-value-container')){
                    tpl.options({
                        ableDropStudentScore: this.getBooleanValue(cell.getData('able-drop-student-score')),
                        ableExemptStudentScore: this.getBooleanValue(cell.getData('able-exempt-student-score'))
                    });
                }else{
                    tpl.options({
                        gradingPeriodId: model.getGradingPeriodId()
                    })
                }

                tpl.renderTo(cell);
            },

            function serializeFromForm(form){
                var o = form.serialize(true);
                var js = new ria.serialize.JsonSerializer();
                return js.deserialize(o, chlk.models.announcement.ShortStudentAnnouncementViewData)
            },

            function fillAll(activeCell, submitCurrent_){
                var form = activeCell.find('form');
                activeCell.removeClass('active-cell');
                activeCell.find('.grade-info').removeClass('empty-grade');
                activeCell.find('.grading-input-popup').hide();
                var model;
                if(submitCurrent_)
                    form.trigger('submit');
                var value = form.find('.value-input').getValue();
                var that = this;
                activeCell.parent('.grade-container').find('.empty-grade').forEach(function(item){
                    var cell = item.parent('.grade-value');
                    var model = that.getModelFromCell(cell);
                    model.setGradeValue(value);
                    that.addFormToActiveCell(cell, model);
                    cell.find('form').trigger('submit');
                });
            },

            function fillAllOneValue(activeCell, value){
                var form = activeCell.find('form');
                activeCell.removeClass('active-cell');
                activeCell.find('.grade-info').removeClass('empty-grade');
                activeCell.find('.grading-input-popup').hide();
                var that = this;
                activeCell.parent('.grade-container').find('.empty-grade').forEach(function(item){
                    var cell = item.parent('.grade-value');
                    var model = that.getModelFromCell(cell);
                    switch(value.toLowerCase()){
                        case Msg.Dropped.toLowerCase(): model.setDropped(true); break;
                        case Msg.Incomplete.toLowerCase(): model.setIncomplete(true); break;
                        case Msg.Late.toLowerCase(): model.setLate(true); break;
                        case Msg.Exempt.toLowerCase(): model.setExempt(true); break;
                        default:{
                            model.setGradeValue(value);
                        }
                    }
                    that.addFormToActiveCell(cell, model);
                    cell.find('form').trigger('submit');
                });
            },

            [ria.mvc.DomEventBind('click', '.fill-grade-container')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function fillGradeClick(node, event){
                var activeCell = node.parent('.active-cell');
                var input = activeCell.find('input[name=gradevalue]');
                var value = input.getValue();
                if(value && !input.hasClass('error') && value.toLowerCase() != 'dropped' && value.toLowerCase() != 'exempt')
                    this.fillAll(activeCell, true);
            },

            [ria.mvc.DomEventBind('click', '.cant-drop:checked')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            Boolean, function cantDropClick(node, event){
                return false;
            },

            [ria.mvc.DomEventBind('click', '.post-gradebook-button')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function postGradeBookClick(node, event){
                jQuery(node.find('a').valueOf()[0]).text('Saving')
            },

            [[Object, String]],
            OVERRIDE, VOID, function onModelReady_(model, msg_) {
                BASE(model, msg_);

                this._lastModel = null;
                if (model instanceof chlk.models.grading.GradingClassSummaryGridForCurrentPeriodViewData)
                    this._lastModel = model;
            },

            [ria.mvc.DomEventBind('click', '[data-sort-type]')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function sortByAnnoClick($node, event){
                Assert(this._lastModel instanceof chlk.models.grading.GradingClassSummaryGridForCurrentPeriodViewData)
                if (this._lastModel == null) return;

                _DEBUG && console.time('sorting');

                var ordered,
                    multiplier = -1,
                    sortMode = $node.getData('sort-type'),
                    sortOrder = $node.getData('sort-order');
                switch (sortMode) {
                    case 'name':
                        multiplier = 1;
                        ordered = this._lastModel.getCurrentGradingGrid().getStudents()
                            .map(function (_) { return [_.getStudentInfo().getId(), _.getStudentInfo().getLastName()] })
                        break;

                    case 'avg':
                        var avgIndex = $node.getData('sort-avg-index') | 0;
                        ordered = this._lastModel.getCurrentGradingGrid().getStudentAverages()[avgIndex].getAverages()
                            .map(function (_) {
                                var value = _.isExempt() ? -1 : _.getEnteredAvg() != null ? _.getEnteredAvg() : _.getCalculatedAvg();
                                return [_.getStudentId(), value];
                            })
                        break;

                    case 'total':
                        ordered = this._lastModel.getCurrentGradingGrid().getStudentTotalPoints()
                            .map(function (_) {
                                return [_.getStudentId(), _.getTotalPoint() + _.getMaxTotalPoint()];
                            })
                        break;

                    case 'anno':
                        var annoId = chlk.models.id.AnnouncementId($node.getData('sort-anno-id') | 0);
                        ordered = this._lastModel.getCurrentGradingGrid()
                            .getGradingItems()
                            .filter(function (_) { return _.getId() == annoId })
                            [0]
                            .getStudentAnnouncements()
                            .getItems()
                            .map(function (_, index) { return [_.getStudentId(), _.getNumericGradeValue(), index]; });
                        break;
                    default:
                        return;
                }

                ordered = ordered.sort(function (_1, _2) { return multiplier * strcmp(_1, _2); });

                if (sortOrder == 'asc')
                    ordered = ordered.reverse();

                var remap = {};
                ordered.forEach(function (item, index) { remap[item[0]] = index; });

                this._lastModel.getCurrentGradingGrid()
                    .setStudents(reorder(
                        this._lastModel.getCurrentGradingGrid().getStudents(),
                        remap,
                        function (_) { return _.getStudentInfo().getId() }
                    ));

                var totals = this._lastModel.getCurrentGradingGrid().getStudentTotalPoints();
                totals && this._lastModel.getCurrentGradingGrid()
                    .setStudentTotalPoints(reorder(
                        totals,
                        remap,
                        function (_) { return _.getStudentId() }
                    ));

                this._lastModel.getCurrentGradingGrid().getStudentAverages()
                    .forEach(function (_) {
                        _.setAverages(reorder(
                            _.getAverages(),
                            remap,
                            function (_) { return _.getStudentId() }
                        ))
                    })

                this._lastModel.getCurrentGradingGrid().getGradingItems()
                    .forEach(function (_) {
                        _.getStudentAnnouncements().setItems(reorder(
                            _.getStudentAnnouncements().getItems(),
                            remap,
                            function (_) { return _.getStudentId() }
                        ))
                    })

                _DEBUG && console.timeEnd('sorting');

                _DEBUG && console.time('repainting');

                var pIndex = chlk.controls.LeftRightToolbarControl.GET_CURRENT_PAGE(this.dom.find('.ann-types-container .grid-toolbar'));
                this.refreshD(ria.async.Future.$fromData(this._lastModel))
                    .then(function () {
                        var node = this.dom.find('.ann-types-container .grid-toolbar');
                        chlk.controls.LeftRightToolbarControl.SET_CURRENT_PAGE(node, pIndex);

                        setTimeout(function () {
                            this.dom.find('.transparent-container').removeClass('transparent-container').removeClass('delay');

                            this.dom.find('[data-sort-type][data-sort-order]').removeData('sort-order');
                            var newSortOrder = sortOrder == 'asc' ? 'desc' : 'asc';

                            switch(sortMode) {
                                case 'name':
                                    this.dom.find('[data-sort-type=name]').setData('sort-order', newSortOrder); break;
                                case 'avg':
                                    this.dom.find('[data-sort-type=avg][data-sort-avg-index=' + avgIndex + ']').setData('sort-order', newSortOrder); break;
                                case 'total':
                                    this.dom.find('[data-sort-type=total]').setData('sort-order', newSortOrder); break;
                                case 'anno':
                                    this.dom.find('[data-sort-type=anno][data-sort-anno-id=' + annoId.valueOf() + ']').setData('sort-order', newSortOrder); break;
                            }
                        }.bind(this), 17);

                        _DEBUG && console.timeEnd('repainting');
                    }, this);
            }
        ]);
});