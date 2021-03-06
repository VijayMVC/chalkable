REQUIRE('chlk.templates.grading.GradingClassStandardsGridTpl');
REQUIRE('chlk.templates.grading.TeacherClassGradingGridStandardsItemTpl');
REQUIRE('chlk.templates.grading.ShortGradingClassStandardsGridItemsTpl');
REQUIRE('chlk.templates.grading.StandardsInputTpl');
REQUIRE('chlk.activities.common.InfoByMpPage');
REQUIRE('chlk.models.grading.GradingClassSummary');
REQUIRE('chlk.activities.grading.BaseGridPage');
REQUIRE('chlk.templates.grading.GradingStandardsPopUpTpl');
REQUIRE('chlk.templates.grading.StandardsPopupTpl');
REQUIRE('chlk.templates.grading.StandardsPopupItemsTpl');

NAMESPACE('chlk.activities.grading', function () {

    /** @class chlk.activities.grading.GradingClassStandardsGridPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.grading.GradingClassStandardsGridTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.grading.GradingCommentsTpl, chlk.activities.lib.DontShowLoader(), '.grading-comments-list', ria.mvc.PartialUpdateRuleActions.Replace)],
        'GradingClassStandardsGridPage', EXTENDS(chlk.activities.grading.BaseGridPage), [

            [ria.mvc.DomEventBind('mouseenter', '.popup-on-hover')],
            [[ria.dom.Dom, ria.dom.Event]],
            function cellHover(node, event){
                if(!node.hasClass('active')){
                    this.dom.find('.popup-on-hover.active').removeClass('active');
                    node.addClass('active');

                    var infoNode = node.find('.grade-info'),
                        studentId = infoNode.getData('studentid'),
                        standardId = infoNode.getData('standardid');

                    var model = new chlk.models.grading.StandardsPopupViewData(new chlk.models.id.SchoolPersonId(studentId), new chlk.models.id.StandardId(standardId));
                    var tpl = new chlk.templates.grading.StandardsPopupTpl();
                    tpl.assign(model);

                    ria.dom.Dom('.student-standard-popup').removeSelf();
                    var dom = new ria.dom.Dom().fromHTML(tpl.render());
                    dom.appendTo(ria.dom.Dom('body'));

                    var popUp = ria.dom.Dom('.student-standard-popup'),
                        left = node.offset().left - (popUp.find('.popup-bubble').width() - node.width())/2;
                    popUp.setCss('left', left);
                    popUp.setCss('top', node.offset().top + node.height()).show();

                    node.find('.show-popup').trigger('click');
                }
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.grading.StandardsPopupItemsTpl, 'popup-items')],
            VOID, function updateStandardPopup(tpl, model, msg_) {
                var studentId = model.getStudentId(),
                    standardId = model.getStandardId(),
                    target = new ria.dom.Dom(".student-standard-popup-" + studentId.valueOf() + '-' + standardId.valueOf());

                if(target.exists()){
                    var cnt = target.find(".items-cnt").addClass("processed");
                    tpl.renderTo(cnt.empty());
                }
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.grading.ShortGradingClassStandardsGridItemsTpl)],
            VOID, function updateGradingPeriodPart(tpl, model, msg_) {
                this.updateGradingPeriodPartRule_(tpl, model);
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.grading.TeacherClassGradingGridStandardsItemTpl)],
            VOID, function updateGrade(tpl, model, msg_) {
                var container = this.dom.find('.grade-value[data-student-id=' + model.getStudentId().valueOf() +
                    '][data-standard-id=' + model.getStandardId().valueOf() +
                    '][data-grading-period-id=' + model.getGradingPeriodId().valueOf() + ']');
                container.empty();
                container.removeClass('active-cell');
                tpl.options({
                    ableEdit: true
                });
                tpl.renderTo(container);
                if(!this.dom.find('.active-cell').exists())
                    container.find('.grade-info').trigger('click');
            },

            OVERRIDE, function prepareAllScores(model){
                BASE(model);
                var allScores = [];
                model.getAlphaGrades().forEach(function(item){
                    allScores.push([item.getName(), item.getId()]);
                    allScores.push([item.getName() + ' (fill all)', item.getId()]);
                });
                this.setAllScores(allScores);
            },

            function afterCellShow(parent){
                var button = parent.parent('.marking-period-container').find('.comment-button');
                var gradeId = parent.find('[name=gradeid]').getValue(), showComment;
                this.getAllScores().forEach(function(score){
                    if(score[1].valueOf() == gradeId)
                        showComment = true;
                });
                if(showComment)
                    button.show();
                else
                    button.hide();
            },

            OVERRIDE, function beforeFormSubmit_(form, value, model, isAvg_){
                BASE(form, value, model, isAvg_);
                var gradeId;
                this.getAllScores().forEach(function(score){

                    //Need if scores are int. And user input is like: 2.00
                    var scoreInt = parseInt(score[0]);
                    var valueInt = parseInt(value);

                    if(!isNaN(scoreInt) && !isNaN(valueInt)) {
                        if (scoreInt === valueInt)
                            gradeId = score[1].valueOf();
                    } else
                    {
                        if(score[0].toLowerCase() == value.toLowerCase())
                            gradeId = score[1].valueOf();
                    }
                });

                form.find('input[name=gradeid]').setValue(gradeId || '');
                if(this._lastModel){
                    var items = this._lastModel.getCurrentGradingGrid().getGradingItems(),
                        len = items.length;

                    for(var i=0; i<len; i++){
                        var item = items[i];
                        if(item.standard.standardid == model.getStandardId().valueOf()){
                            var standards = item.items;
                            for(var j=0; j<standards.length; j++){
                                var standard = standards[j];
                                if(standard.standardid == model.getStandardId().valueOf()){
                                    if(gradeId)
                                        standard.gradeid = gradeId;
                                    standard.gradevalue = model.getGradeValue();
                                    standard.comment = model.getComment();
                                    break;
                                }
                            }
                            break;
                        }
                    }
                }

            },

            [[ria.dom.Dom]],
            Object, function getModelFromCell(cell){
                var node = cell.find('.grade-info'), model;
                var grade = cell.find('.grade-text').getData('grade-value'),
                    comment = node.getData('comment');
                grade = grade ? grade.toString() : '';
                comment = comment ? comment.toString() : '';
                model = new chlk.models.standard.StandardGrading(
                    grade,
                    new chlk.models.id.StandardId(node.getData('standardid')),
                    new chlk.models.id.GradingPeriodId(node.getData('gradingperiodid')),
                    new chlk.models.id.SchoolPersonId(node.getData('studentid')),
                    new chlk.models.id.ClassId(node.getData('classid')),
                    comment,
                    new chlk.models.id.GradeId(node.getData('gradeid'))
                );

                return model;
            },

            OVERRIDE, function addPopUpByModel(cell, model){
                BASE(cell, model);
                var popUpTpl = new chlk.templates.grading.GradingStandardsPopUpTpl();
                popUpTpl.assign(model);
                this.dom.find('#grading-popup').setHTML(popUpTpl.render());
            },

            OVERRIDE, function prepareTplForForm_(cell, model){
                return new chlk.templates.grading.StandardsInputTpl();
            },

            OVERRIDE, function getFormModelClass_(){
                return chlk.models.standard.StandardGrading
            },

            OVERRIDE, VOID, function onRender_(model){
                BASE(model);

                new ria.dom.Dom().on('click.standard', function(doc, event){
                    var node = new ria.dom.Dom(event.target);

                    if(!node.isOrInside('.student-standard-popup')){
                        ria.dom.Dom('.student-standard-popup').removeSelf();
                    }
                });
            },

            OVERRIDE, VOID, function onStop_() {
                BASE();
                new ria.dom.Dom().off('click.standard');
                ria.dom.Dom('.student-standard-popup').removeSelf();
            },

            [[Object, String]],
            OVERRIDE, VOID, function onModelReady_(model, msg_) {
                BASE(model, msg_);

                if (model instanceof chlk.models.classes.BaseClassProfileViewData)
                    this._lastModel = model.getClazz().getGradingPart();

                if (model instanceof chlk.models.grading.GradingClassStandardsGridForCurrentPeriodViewData)
                    this._lastModel = model;

                if (model instanceof chlk.models.grading.GradingClassSummaryGridItems && this._lastModel) {
                    this._lastModel.setCurrentGradingGrid(model);
                    this._lastModel.setGradingPeriodId(chlk.models.id.GradingPeriodId(model.getGradingPeriod().getId()));
                }

                if (model instanceof chlk.models.standard.StandardGrading && this._lastModel) {
                    this._lastModel.getCurrentGradingGrid().getGradingItems()
                        .filter(function (_) {
                            return _.standard.standardid == model.getStandardId().valueOf();
                        })
                        .forEach(function (_) {
                            _.items = _.items.map(function (_) {
                                var o = {
                                    gradingperiodid: model.getGradingPeriodId().valueOf(),
                                    standardid: model.getStandardId().valueOf(),
                                    gradeid: model.getGradeId() && model.getGradeId().valueOf(),
                                    studentid: model.getStudentId().valueOf(),
                                    classid: model.getClassId().valueOf(),
                                    gradevalue: model.getGradeValue(),
                                    comment: model.getComment(),
                                    numericgrade: model.getNumericGrade()
                                };
                                return _.studentid == model.getStudentId().valueOf() ? o : _;
                            })
                        });
                }
            },

            [ria.mvc.DomEventBind('click', '[data-sort-type]')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function sortByAnnoClick($node, event){
                Assert(this._lastModel instanceof chlk.models.grading.GradingClassStandardsGridForCurrentPeriodViewData);
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

                    case 'standard':
                        var sdId = $node.getData('sort-id') | 0;
                        ordered = this._lastModel.getCurrentGradingGrid()
                            .getGradingItems()
                            .filter(function (_) { return _.standard.standardid == sdId })
                            [0]
                            .items
                            .map(function (_, index) { return [new chlk.models.id.SchoolPersonId(_.studentid), _.gradevalue, index]; });
                        break;
                    default:
                        return;
                }

                ordered = ordered.sort(function (_1, _2) { return multiplier * that.strcmp_(_1, _2); });

                if (sortOrder == 'asc')
                    ordered.reverse();

                var remap = {};
                ordered.forEach(function (item, index) { remap[item[0]] = index; });

                this._lastModel.getCurrentGradingGrid()
                    .setStudents(this.reorder_(
                        this._lastModel.getCurrentGradingGrid().getStudents(),
                        remap,
                        function (_) { return _.getStudentInfo().getId() }
                    ));

                this._lastModel.getCurrentGradingGrid().getGradingItems()
                    .forEach(function (_) {
                        _.items = that.reorder_(
                            _.items,
                            remap,
                            function (_) { return new chlk.models.id.SchoolPersonId(_.studentid) }
                        )
                    });

                _DEBUG && console.timeEnd('sorting');

                _DEBUG && console.time('repainting');

                var pIndex = chlk.controls.LeftRightToolbarControl.GET_CURRENT_PAGE(this.dom.find('.ann-types-container .grid-toolbar'));
                this.refreshD(ria.async.Future.$fromData(this._lastModel))
                    .then(function () {
                        var node = this.dom.find('.ann-types-container .grid-toolbar');
                        chlk.controls.LeftRightToolbarControl.SET_CURRENT_PAGE(node, pIndex);

                        setTimeout(function () {
                            this.dom.find('.last-container').removeClass('last-container').removeClass('delay');

                            this.dom.find('[data-sort-type][data-sort-order]').removeData('sort-order');
                            var newSortOrder = sortOrder == 'asc' ? 'desc' : 'asc';

                            switch(sortMode) {
                                case 'name':
                                    this.dom.find('[data-sort-type=name]').setData('sort-order', newSortOrder); break;
                                case 'standard':
                                    this.dom.find('[data-sort-type=standard][data-sort-id=' + sdId + ']').setData('sort-order', newSortOrder); break;
                            }
                        }.bind(this), 17);

                        _DEBUG && console.timeEnd('repainting');
                    }, this);
            }
        ]);
});