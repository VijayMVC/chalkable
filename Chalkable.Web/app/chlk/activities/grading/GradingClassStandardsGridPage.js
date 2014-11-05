REQUIRE('chlk.templates.grading.GradingClassStandardsGridTpl');
REQUIRE('chlk.templates.grading.TeacherClassGradingGridStandardsItemTpl');
REQUIRE('chlk.templates.grading.ShortGradingClassStandardsGridItemsTpl');
REQUIRE('chlk.activities.common.InfoByMpPage');
REQUIRE('chlk.models.grading.GradingClassSummary');

NAMESPACE('chlk.activities.grading', function () {

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

    var gradingGridTimer;

    /** @class chlk.activities.grading.GradingClassStandardsGridPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.grading.GradingClassStandardsGridTpl)],
        'GradingClassStandardsGridPage', EXTENDS(chlk.activities.lib.TemplatePage), [
            Array, 'allScores',

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

            function openGradingPeriod(container){
                container.parent('.marking-period-container').addClass('open');
                var annContainer = container.find('.ann-types-container');
                container.setCss('height', 0);
                jQuery(container.valueOf()).animate({
                    height: (annContainer.height() + parseInt(annContainer.getCss('margin-bottom'), 10))
                }, 500);
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.grading.ShortGradingClassStandardsGridItemsTpl)],
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
                    parent.find('.mp-title').setData('tooltip', tooltipText);
                }.bind(this), 1);

            },

            chlk.models.id.ClassId, 'classId',

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

            [ria.mvc.DomEventBind('click', '.comment-button')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function commentBtnClick(node, event){
                var active = this.dom.find('.active-cell');
                var popUp = this.dom.find('.chlk-pop-up-container.comment');
                var main = this.dom.parent('#main');
                var bottom = main.height() + main.offset().top - active.offset().top + 73;
                var left = active.offset().left - main.offset().left - 54;
                popUp.setCss('bottom', bottom);
                popUp.setCss('left', left);
                var comment = active.find('.comment-value').getValue();
                popUp.find('textarea').setValue(comment);
                popUp.show();
                setTimeout(function(){
                    popUp.find('.comment-value').trigger('focus');
                }, 1)
            },

            [ria.mvc.DomEventBind('click', '.add-comment')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function addCommentBtnClick(node, event){
                var cell = this.dom.find('.active-cell');
                var commentInput = cell.find('.comment-value');
                var comment = node.parent('.chlk-pop-up-container').find('textarea').getValue();
                commentInput.setValue(comment);
                node.parent('.chlk-pop-up-container.comment').hide();
                setTimeout(function(){
                    cell.find('form').trigger('submit');
                    cell.find('.value-input').trigger('focus');
                }, 1);
                //this.setValue(cell, true);
            },

            [ria.mvc.DomEventBind(chlk.controls.LRToolbarEvents.AFTER_RENDER.valueOf(), '.grid-toolbar')],
            [[ria.dom.Dom, ria.dom.Event]],
            function afterTbRender(node, event){
                this.beforeTbAnimation(node);
            },

            Number, function getColumns(){
                return 6;
            },

            [ria.mvc.DomEventBind(chlk.controls.LRToolbarEvents.BEFORE_ANIMATION.valueOf(), '.grid-toolbar')],
            [[ria.dom.Dom, ria.dom.Event, Boolean, Number]],
            function beforeTbAnimation(toolbar, event_, isLeft_, index_){
                var num = this.getColumns();
                this.dom.find('.transparent-container').removeClass('transparent-container').removeClass('delay');
                var startIndex = index_ ? index_ * num + num : num;
                var node = toolbar.find('.dotted-container:eq(' + startIndex + ')');
                if(!node.is(':last-child')){
                    if(isLeft_)
                        node.addClass('delay');
                    setTimeout(function(){
                        node.addClass('transparent-container');
                    },1);
                }
            },

            [ria.mvc.DomEventBind('keydown', '.grade-autocomplete')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function gradeKeyDown(node, event){
                var list = this.dom.find('.autocomplete-list:visible'),
                    cell = node.parent('.active-cell');
                if(event.keyCode == ria.dom.Keys.ENTER.valueOf()){
                    if(!node.hasClass('error') && !node.hasClass('blocked')){
                        if(list.exists()){
                            if(list.find('.see-all').hasClass('hovered')){
                                list.find('.see-all').trigger('click');
                                return false;
                            }
                            var hovered = list.find('.hovered');
                            if(hovered.exists()){
                                node.setValue(hovered.getHTML());
                                node.parent('form').find('input[name=gradeid]').setValue(hovered.getData('id'));
                            }
                        }
                        this.setValue(cell);

                    }
                }

                setTimeout(function(node, event){
                    var isDown = event.keyCode == ria.dom.Keys.DOWN.valueOf();
                    var isUp = event.keyCode == ria.dom.Keys.UP.valueOf();
                    if((isDown || isUp) && list.exists()){
                        var hovered = list.find('.hovered');
                        if(hovered.exists()){
                            if(isDown && hovered.next().exists()){
                                hovered.removeClass('hovered');
                                hovered.next().addClass('hovered');
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
                    }
                }.bind(this, node, event), 10);
                return true;
            },

            [ria.mvc.DomEventBind('dblclick', '.grade-autocomplete')],
            [[ria.dom.Dom, ria.dom.Event]],
            function inputDblClickClick(node, event){
                node.removeClass('not-equals');
                this.updateDropDown(this.getAllScores(), node, true);
            },

            [ria.mvc.DomEventBind('focus', '.grade-autocomplete')],
            [[ria.dom.Dom, ria.dom.Event]],
            function gradeFocus(node, event){
                node.select();
            },

            [ria.mvc.DomEventBind('keydown', '.grade-autocomplete')],
            [[ria.dom.Dom, ria.dom.Event]],
            function gradeUpDownKeyDown(node, event){
                var list = this.dom.find('.autocomplete-list:visible');
                var curCell = node.parent('.grade-value'), cell, curBlock, valueInput;
                if(!list.is(':visible')){
                    switch(event.keyCode){
                        case ria.dom.Keys.UP.valueOf():
                            cell = curCell.previous('.grade-value');
                            break;
                        case ria.dom.Keys.DOWN.valueOf():
                            cell = curCell.next('.grade-value');
                            break;
                    }
                }

                switch(event.keyCode){
                    case ria.dom.Keys.LEFT.valueOf():
                        valueInput = curCell.find('.value-input');
                        if(valueInput.getSelectedText() || valueInput.getCursorPosition() == 0){
                            curBlock = curCell.parent('.grade-container').previous('.grade-container');
                            var nameContainer = curBlock.parent('.ann-types-container').find('.name-container');
                            if(curBlock.exists() && curBlock.offset().left >= (nameContainer.offset().left + nameContainer.width()))
                                cell = curBlock.find('.grade-value[row-index=' + curCell.getAttr('row-index') + ']');
                        }
                        break;
                    case ria.dom.Keys.RIGHT.valueOf():
                        valueInput = curCell.find('.value-input');
                        var value = valueInput.getValue() || '';
                        if(valueInput.getSelectedText() || valueInput.getCursorPosition() == value.length){
                            curBlock = curCell.parent('.grade-container').next('.grade-container:not(.transparent-container)');
                            if(curBlock.exists())
                                cell = curBlock.find('.grade-value[row-index=' + curCell.getAttr('row-index') + ']');
                        }
                        break;
                }
                if(cell && cell.exists() && cell.hasClass('gradable')){
                    this.submitActiveForm();
                    this.showCell(cell);
                }
            },

            [ria.mvc.DomEventBind('keyup', '.grade-autocomplete')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function gradeKeyUp(node, event){
                var suggestions = [], cell = node.parent('.active-cell');
                var isDown = event.keyCode == ria.dom.Keys.DOWN.valueOf();
                var isUp = event.keyCode == ria.dom.Keys.UP.valueOf();
                var list = this.dom.find('.autocomplete-list:visible');
                var value = node.getValue();
                if(!value){
                    node.addClass('empty-grade');
                    node.removeClass('error')
                        .removeClass('blocked');;
                }
                else{
                    node.removeClass('empty-grade');
                }
                if(value && !isDown && !isUp){
                    if(event.keyCode == ria.dom.Keys.ENTER.valueOf()){
                        return false;
                    }else{
                        var text = node.getValue() ? node.getValue().trim() : '';
                        suggestions = text  ? this.getSuggestedValues(text) : [];
                        if(!suggestions.length)
                            node.addClass('error');
                        else
                            node.removeClass('error');
                        var p = false;
                        suggestions.forEach(function(item){
                            if(text.toLowerCase()==item[0].toLowerCase()){
                                node.parent('form').find('input[name=gradeid]').setValue(item[1].valueOf());
                                p = true;
                            }
                        });
                        if(p)
                            node.removeClass('blocked');
                        else
                            node.addClass('blocked');
                        this.updateDropDown(suggestions, node);
                    }
                    this.updateDropDown(suggestions, node);
                }
                return true;
            },

            function setValue(node, isComment_){
                var activeCell = node;
                activeCell.find('form').trigger('submit');
                var nextCell = activeCell.next().find('.edit-cell');
                if(nextCell.exists() && !isComment_)
                    nextCell.trigger('click');
                else
                    this.dom.trigger('click');
            },

            [[String]],
            Array, function getSuggestedValues(text){
                var text = text.toLowerCase();
                var res = [];
                this.getAllScores().forEach(function(item){
                    if(item[0].toLowerCase().indexOf(text) == 0)
                        res.push(item);
                });
                return res;
            },

            [ria.mvc.DomEventBind('mouseover', '.autocomplete-item')],
            [[ria.dom.Dom, ria.dom.Event]],
            function itemHover(node, event){
                if(!node.hasClass('hovered'))
                    node.parent().find('.hovered').removeClass('hovered');
                node.addClass('hovered');
            },

            [ria.mvc.DomEventBind('click', '.see-all')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function seeAllClick(node, event){
                this.updateDropDown(this.getAllScores(), this.dom.find('.active-cell').find('.value-input'), true);
                return false;
            },

            [ria.mvc.DomEventBind('click', '.autocomplete-item:not(.see-all)')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function listItemBtnClick(node, event){
                var cell = this.dom.find('.active-cell');
                cell.find('.error').removeClass('error');
                var input = cell.find('.grade-autocomplete');
                input.setValue(node.getHTML());
                input.parent('form').find('input[name=gradeid]').setValue(node.getData('id'));
                this.setValue(cell);
                return false;
            },

            VOID, function updateDropDown(suggestions, node, all_){
                var list = this.dom.find('.autocomplete-list');
                if(suggestions.length || node.hasClass('error')){
                    var html = '';
                    suggestions.forEach(function(item, index){
                        html+='<div class="autocomplete-item" data-id="' + item[1] + '">' + item[0] + '</div>'
                    });
                    if(!all_)
                        html += '<div class="autocomplete-item see-all">See all »</div>';
                    var top = node.offset().top - list.parent().offset().top + node.height() + 43;
                    var left = node.offset().left - list.parent().offset().left + 61;
                    list.setCss('top', top)
                        .setCss('left', left);
                    list.setHTML(html)
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

            OVERRIDE, VOID, function onRender_(model){
                BASE(model);
                this.setClassId(model.getTopData().getSelectedItemId());
                this.prepareAllScores(model);
                this.addEvents();
            },

            OVERRIDE, VOID, function onStop_() {
                BASE();
                new ria.dom.Dom().off('click.grading_popup');
                new ria.dom.Dom().off('click.grade');
            },

            function prepareAllScores(model){
                var allScores = [];
                model.getAlphaGrades().forEach(function(item){
                    allScores.push([item.getName(), item.getId()]);
                });
                this.setAllScores(allScores);
            },

            function submitActiveForm(){
                var activeCell = this.dom.find('.active-cell');
                if(activeCell.exists()){
                    setTimeout(function(){
                        activeCell.find('form').trigger('submit');
                    },1)
                }
                activeCell.removeClass('active-cell');
            },

            function showCell(parent){
                if(parent.exists()){
                    if(!parent.hasClass('active-row')){
                        var index = parent.getAttr('row-index');
                        this.dom.find('.active-row').removeClass('active-row');
                        this.dom.find('[row-index=' + index + ']').addClass('active-row');
                    }
                    parent.addClass('active-cell');
                    parent.parent('.marking-period-container').find('.comment-button').show();
                    setTimeout(function(){
                        parent.find('input').trigger('focus');
                    },1)
                }else{
                    this.dom.find('.active-row').removeClass('active-row');
                    this.dom.find('.comment-button').hide();
                }
            },

            function addEvents(){
                var dom = this.dom, that = this;
                new ria.dom.Dom().on('click.grade', function(doc, event){

                    var node = new ria.dom.Dom(event.target);
                    var popUp = dom.find('.chlk-pop-up-container.comment');
                    if(!node.hasClass('grade-autocomplete') && !node.hasClass('arrow')
                        && !node.isOrInside('.chlk-pop-up-container.comment')
                        && !node.isOrInside('.autocomplete-list')){
                            dom.find('.autocomplete-list').setHTML('').hide();
                            if(!node.hasClass('comment-button')){
                                popUp.hide();
                                var parent = node.parent('.grade-value.gradable');
                                that.submitActiveForm();
                                that.showCell(parent);
                            }
                    }
                });
            },

            [ria.mvc.DomEventBind('submit', '.grade-container form')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function submitForm(node, event){
                var res = !node.find('input[name="gradevalue"]').hasClass('error');
                var valueInput = node.find('.value-input'), value = valueInput.getValue() || '';
                var commentInput = node.find('.comment-value');
                if(!res || (value.toLowerCase() == (valueInput.getData('value') || '').toLowerCase()
                    && (commentInput.getValue() || '') == (commentInput.getData('comment') || '')))
                        return false;
                if(!value)
                    node.find('input[name=gradeid]').setValue('');
                node.parent().find('.value').setHTML('...');
                return true;
            },

            [[Object, String]],
            OVERRIDE, VOID, function onModelReady_(model, msg_) {
                BASE(model, msg_);

                this._lastModel = null;
                if (model instanceof chlk.models.grading.GradingClassStandardsGridForCurrentPeriodViewData)
                    this._lastModel = model;
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
                    sortOrder = $node.getData('sort-order');
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

                ordered = ordered.sort(function (_1, _2) { return multiplier * strcmp(_1, _2); });

                if (sortOrder == 'asc')
                    ordered.reverse();

                var remap = {};
                ordered.forEach(function (item, index) { remap[item[0]] = index; });

                this._lastModel.getCurrentGradingGrid()
                    .setStudents(reorder(
                        this._lastModel.getCurrentGradingGrid().getStudents(),
                        remap,
                        function (_) { return _.getStudentInfo().getId() }
                    ));

                this._lastModel.getCurrentGradingGrid().getGradingItems()
                    .forEach(function (_) {
                        _.setItems(reorder(
                            _.getItems(),
                            remap,
                            function (_) { return _.getStudentId() }
                        ))
                    });

                _DEBUG && console.timeEnd('sorting');

                _DEBUG && console.time('repainting');

                var pIndex = chlk.controls.LeftRightToolbarControl.GET_CURRENT_PAGE(this.dom.find('.ann-types-container .grid-toolbar'));
                console.info(pIndex);
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