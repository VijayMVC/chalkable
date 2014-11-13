REQUIRE('chlk.templates.grading.GradingClassSummaryGridTpl');
REQUIRE('chlk.templates.grading.GradingInputTpl');
REQUIRE('chlk.templates.grading.ShortGradingClassSummaryGridItemsTpl');
REQUIRE('chlk.templates.grading.TeacherClassGradingGridSummaryCellTpl');
REQUIRE('chlk.templates.grading.ShortGradingClassSummaryGridAvgsTpl');
REQUIRE('chlk.templates.grading.GradingCommentsTpl');
REQUIRE('chlk.templates.grading.StudentAverageTpl');
REQUIRE('chlk.templates.grading.AvgCodesPopupTpl');
REQUIRE('chlk.templates.grading.StudentAverageInputTpl');

REQUIRE('chlk.activities.grading.BaseGridPage');

REQUIRE('chlk.models.common.Array');
REQUIRE('chlk.models.announcement.ShortStudentAnnouncementViewData');
REQUIRE('chlk.models.grading.AvgCodesPopupViewData');

NAMESPACE('chlk.activities.grading', function () {

    var gradingGridTimer;

    /** @class chlk.activities.grading.GradingClassSummaryGridPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.grading.GradingClassSummaryGridTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.grading.GradingCommentsTpl, chlk.activities.lib.DontShowLoader(), '.grading-comments-list', ria.mvc.PartialUpdateRuleActions.Replace)],
        'GradingClassSummaryGridPage', EXTENDS(chlk.activities.grading.BaseGridPage), [

            Array, 'standardScores',

            ArrayOf(chlk.models.grading.AvgComment), 'gradingComments',

            [[ria.dom.Dom]],
            OVERRIDE, function loadGradingPeriod(container){
                clearTimeout(gradingGridTimer);
                BASE(container);
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

            OVERRIDE, function getScores_(node_){
                var parent = node_.parent('.grade-value'), scores;
                if(parent.hasClass('avg-value-container')){
                    scores = this.getStandardScores();
                    if(parent.find('.grade-info').getData('may-be-exempt'))
                        scores = scores.concat([['Exempt', '']]);
                }else{
                    scores = this.getAllScores();
                    if(parent.getData('able-drop-student-score'))
                        scores = scores.concat([['Dropped', ''], ['Dropped (fill all)', '']]);
                    if(parent.getData('able-exempt-student-score'))
                        scores = scores.concat([['Exempt', ''], ['Exempt (fill all)', '']]);
                }

                return scores;
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.grading.ShortGradingClassSummaryGridItemsTpl)],
            VOID, function updateGradingPeriodPart(tpl, model, msg_) {
                this.updateGradingPeriodPartRule_(tpl, model);
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

            [[String, ria.dom.Dom]],
            OVERRIDE, function updateInputByText(text, node){
                var parsed = parseFloat(text);
                if(parsed || parsed == 0){
                    node.removeClass('error');
                    if(parsed != text || parsed > 9999.99 || parsed < -9999.99){
                        node.addClass('error');
                    }else{
                        this.hideDropDown();
                    }
                    return [];
                }
                return BASE(text, node);
            },

            [[ria.dom.Dom]],
            OVERRIDE, function afterGradeKeyUp_(cell){
                BASE(cell);
                setTimeout(function(cell){
                    var node = cell.find('.grade-autocomplete');
                    var numericValue = parseFloat(node.getValue());
                    if(numericValue == node.getValue()){
                        var model = this.getModelFromCell(cell);
                        this.updateFlagByModel(model, cell);
                    }
                }.bind(this, cell), 10);
            },

            OVERRIDE, function updateFlagByModel(model, cell){
                BASE(model, cell);
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

            [ria.mvc.DomEventBind('click', '.grading-select + .chzn-container')],
            [[ria.dom.Dom, ria.dom.Event]],
            function clickSelect(node, event){
                event.stopPropagation();
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

            OVERRIDE, function beforeFormSubmit_(form, value, isAvg_){
                BASE(form, value, isAvg_);
                var autoUpdateForm = form.parent('.marking-period-container').find('.load-grading-period');
                clearTimeout(gradingGridTimer);
                gradingGridTimer = setTimeout(function(){
                    autoUpdateForm.find('.auto-update').setValue(true);
                    autoUpdateForm.find('.avg-value').setValue(isAvg_);
                    autoUpdateForm.trigger('submit');
                    setTimeout(function(){
                        autoUpdateForm.find('.auto-update').setValue(false);
                        autoUpdateForm.find('.avg-value').setValue(false);
                    }, 1);
                }, 5000);
            },

            OVERRIDE, function prepareAllScores(model){
                BASE(model);
                var allScores = [], standardScores = [];
                 model.getAlternateScores().forEach(function(item){
                    allScores.push([item.getName(), '']);
                    allScores.push([item.getName() + ' (fill all)', '']);
                });
                model.getAlphaGrades().forEach(function(item){
                    allScores.push([item.getName(), '']);
                    standardScores.push([item.getName(), '']);
                    allScores.push([item.getName() + ' (fill all)', '']);
                });
                allScores = allScores.concat([['Incomplete', ''], ['Incomplete (fill all)', ''], ['Late', ''], ['Late (fill all)', '']]);
                this.setAllScores(allScores);
                this.setStandardScores(standardScores);
                this.setGradingComments(model.getGradingComments());
            },

            function afterCellShow(parent){
                var mp = parent.parent('.marking-period-container');
                if(!parent.hasClass('avg-value-container')){
                    mp.find('.comment-button').show();
                    mp.find('.codes-button').hide();
                }else{
                    var value = parent.find('.grade-info').getData('codes-string');
                    if(value && value.length)
                        mp.find('.codes-button').show();
                    mp.find('.comment-button').hide();
                }
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
                        this.getBooleanValue_(node.getData('may-be-exempt')),
                        this.getBooleanValue_(node.getData('exempt')),
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
                        this.getBooleanValue_(node.getData('dropped')),
                        this.getBooleanValue_(node.getData('islate')),
                        this.getBooleanValue_(node.getData('isexempt')),
                        this.getBooleanValue_(node.getData('isabsent')),
                        this.getBooleanValue_(node.getData('isincomplete')),
                        node.getData('comment'),
                        grade,
                        this.getBooleanValue_(node.getData('isincludeinaverage'))
                    );
                }

                return model;
            },

            OVERRIDE, function prepareTplForForm_(cell, model){
                var tpl;

                if(cell.hasClass('avg-value-container')){
                    tpl = new chlk.templates.grading.StudentAverageInputTpl();
                }else{
                    tpl = new chlk.templates.grading.GradingInputTpl();
                }

                if(!cell.hasClass('avg-value-container')){
                    tpl.options({
                        ableDropStudentScore: this.getBooleanValue_(cell.getData('able-drop-student-score')),
                        ableExemptStudentScore: this.getBooleanValue_(cell.getData('able-exempt-student-score'))
                    });
                }else{
                    tpl.options({
                        gradingPeriodId: model.getGradingPeriodId()
                    })
                }

                return tpl;
            },

            [ria.mvc.DomEventBind('click', '.cant-drop:checked')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            Boolean, function cantDropClick(node, event){
                return false;
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
                    sortOrder = $node.getData('sort-order'),
                    that = this;
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

                ordered = ordered.sort(function (_1, _2) { return multiplier * that.strcmp_(_1, _2); });

                if (sortOrder == 'asc')
                    ordered = ordered.reverse();

                var remap = {};
                ordered.forEach(function (item, index) { remap[item[0]] = index; });

                this._lastModel.getCurrentGradingGrid()
                    .setStudents(that.reorder_(
                        this._lastModel.getCurrentGradingGrid().getStudents(),
                        remap,
                        function (_) { return _.getStudentInfo().getId() }
                    ));

                var totals = this._lastModel.getCurrentGradingGrid().getStudentTotalPoints();
                totals && this._lastModel.getCurrentGradingGrid()
                    .setStudentTotalPoints(that.reorder_(
                        totals,
                        remap,
                        function (_) { return _.getStudentId() }
                    ));

                this._lastModel.getCurrentGradingGrid().getStudentAverages()
                    .forEach(function (_) {
                        _.setAverages(that.reorder_(
                            _.getAverages(),
                            remap,
                            function (_) { return _.getStudentId() }
                        ))
                    })

                this._lastModel.getCurrentGradingGrid().getGradingItems()
                    .forEach(function (_) {
                        _.getStudentAnnouncements().setItems(that.reorder_(
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