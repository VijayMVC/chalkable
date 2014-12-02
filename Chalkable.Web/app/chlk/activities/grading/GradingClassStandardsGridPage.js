REQUIRE('chlk.templates.grading.GradingClassStandardsGridTpl');
REQUIRE('chlk.templates.grading.TeacherClassGradingGridStandardsItemTpl');
REQUIRE('chlk.templates.grading.ShortGradingClassStandardsGridItemsTpl');
REQUIRE('chlk.templates.grading.StandardsInputTpl');
REQUIRE('chlk.activities.common.InfoByMpPage');
REQUIRE('chlk.models.grading.GradingClassSummary');
REQUIRE('chlk.activities.grading.BaseGridPage');

NAMESPACE('chlk.activities.grading', function () {

    /** @class chlk.activities.grading.GradingClassStandardsGridPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.grading.GradingClassStandardsGridTpl)],
        'GradingClassStandardsGridPage', EXTENDS(chlk.activities.grading.BaseGridPage), [

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
                tpl.options({
                    ableEdit: true
                });
                tpl.renderTo(container);
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
                parent.parent('.marking-period-container').find('.comment-button').show();
            },

            OVERRIDE, function beforeFormSubmit_(form, value, model, isAvg_){
                BASE(form, value, model, isAvg_);
                var gradeId;
                this.getAllScores().forEach(function(score){
                    if(score[0].toLowerCase() == value.toLowerCase())
                        gradeId = score[1].valueOf()
                });
                gradeId && form.find('input[name=gradeid]').setValue(gradeId);
                if(this._lastModel){
                    var items = this._lastModel.getCurrentGradingGrid().getGradingItems(),
                        len = items.length;

                    for(var i=0; i<len; i++){
                        var item = items[i];
                        if(item.getStandard().getStandardId() == model.getStandardId()){
                            var standards = item.getItems();
                            for(var j=0; j<standards.length; j++){
                                var standard = standards[j];
                                if(standard.getStudentId() == model.getStudentId()){
                                    gradeId && standard.setGradeId(new chlk.models.id.GradeId(gradeId));
                                    standard.setGradeValue(model.getGradeValue());
                                    standard.setComment(model.getComment());
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
                var grade = cell.find('.grade-text').getData('grade-value');
                grade = grade ? grade.toString() : '';
                model = new chlk.models.standard.StandardGrading(
                    grade,
                    new chlk.models.id.StandardId(node.getData('standardid')),
                    new chlk.models.id.GradingPeriodId(node.getData('gradingperiodid')),
                    new chlk.models.id.SchoolPersonId(node.getData('studentid')),
                    new chlk.models.id.ClassId(node.getData('classid')),
                    node.getData('comment')
                );

                return model;
            },

            OVERRIDE, function prepareTplForForm_(cell, model){
                return new chlk.templates.grading.StandardsInputTpl();
            },

            OVERRIDE, function getFormModelClass_(){
                return chlk.models.standard.StandardGrading
            },

            [[Object, String]],
            OVERRIDE, VOID, function onModelReady_(model, msg_) {
                BASE(model, msg_);

                _DEBUG && console.info(model);
                if (model instanceof chlk.models.grading.GradingClassStandardsGridForCurrentPeriodViewData)
                    this._lastModel = model;

                if (model instanceof chlk.models.grading.GradingClassSummaryGridItems && this._lastModel) {
                    this._lastModel.setCurrentGradingGrid(model);
                    this._lastModel.setGradingPeriodId(chlk.models.id.GradingPeriodId(model.getGradingPeriod().getId()));
                }

                if (model instanceof chlk.models.standard.StandardGrading && this._lastModel) {
                    this._lastModel.getCurrentGradingGrid().getGradingItems()
                        .filter(function (_) {
                            return _.getStandard().getStandardId() == model.getStandardId();
                        })
                        .forEach(function (_) {
                            _.setItems(_.getItems().map(function (_) {
                                return _.getStudentId() == model.getStudentId() ? model : _;
                            }))
                        });
                }

                _DEBUG && console.info(this._lastModel);
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
                        var sdId = chlk.models.id.StandardId($node.getData('sort-id') | 0);
                        ordered = this._lastModel.getCurrentGradingGrid()
                            .getGradingItems()
                            .filter(function (_) { return _.getStandard().getStandardId() == sdId })
                            [0]
                            .getItems()
                            .map(function (_, index) { return [_.getStudentId(), _.getGradeValue(), index]; });
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
                        _.setItems(that.reorder_(
                            _.getItems(),
                            remap,
                            function (_) { return _.getStudentId() }
                        ))
                    });

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
                                case 'standard':
                                    this.dom.find('[data-sort-type=standard][data-sort-id=' + sdId.valueOf() + ']').setData('sort-order', newSortOrder); break;
                            }
                        }.bind(this), 17);

                        _DEBUG && console.timeEnd('repainting');
                    }, this);
            }
        ]);
});