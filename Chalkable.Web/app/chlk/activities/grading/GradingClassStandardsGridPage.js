REQUIRE('chlk.templates.grading.GradingClassStandardsGridTpl');
REQUIRE('chlk.templates.grading.TeacherClassGradingGridStandardsItemTpl');
REQUIRE('chlk.activities.common.InfoByMpPage');
REQUIRE('chlk.models.grading.GradingClassSummary');

NAMESPACE('chlk.activities.grading', function () {

    /** @class chlk.activities.grading.GradingClassStandardsGridPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.grading.GradingClassStandardsGridTpl)],
        'GradingClassStandardsGridPage', EXTENDS(chlk.activities.common.InfoByMpPage), [
            Array, 'allScores',

            [ria.mvc.PartialUpdateRule(chlk.templates.grading.TeacherClassGradingGridStandardsItemTpl)],
            VOID, function updateGrade(tpl, model, msg_) {
                var container = this.dom.find('.grade-value[data-student-id=' + model.getStudentId().valueOf() +
                    '][data-standard-id=' + model.getStandardId().valueOf() +
                    '][data-grading-period-id=' + model.getGradingPeriodId().valueOf() + ']');
                container.empty();
                tpl.renderTo(container);
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
                commentInput.setValue(comment).setData('comment', comment);
                node.parent('.chlk-pop-up-container.comment').hide();
                setTimeout(function(){
                    cell.find('input').trigger('focus');
                }, 1);
                this.setValue(cell, true);
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
                    node.removeClass('error');
                }
                else{
                    node.removeClass('empty-grade');
                }
                if(!isDown && !isUp){
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
                this.updateDropDown(this.getAllScores(), this.dom.find('.active-cell'), true);
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

            VOID, function hideDropDown(){
                var list = this.dom.find('.autocomplete-list');
                list.setHTML('')
                    .hide();
            },

            OVERRIDE, VOID, function onRender_(model){
                BASE(model);
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

            function addEvents(){
                var dom = this.dom;
                new ria.dom.Dom().on('click.grade', function(doc, event){

                    var node = new ria.dom.Dom(event.target);
                    if(!node.hasClass('grade-autocomplete') && !node.hasClass('arrow')
                        && !node.isOrInside('.chlk-pop-up-container.comment')
                        && !node.isOrInside('.autocomplete-list')){
                            dom.find('.autocomplete-list').setHTML('').hide();
                            if(!node.hasClass('comment-button')){
                                var parent = node.parent('.grade-value.gradable');
                                dom.find('.active-cell').removeClass('active-cell');
                                if(parent.exists()){
                                    if(!parent.hasClass('active-row')){
                                        var index = parent.getAttr('row-index');
                                        dom.find('.active-row').removeClass('active-row');
                                        dom.find('[row-index=' + index + ']').addClass('active-row');
                                    }
                                    parent.addClass('active-cell');
                                    //node.parent('.marking-period-container').find('.comment-button').show();
                                    setTimeout(function(){
                                        parent.find('input').trigger('focus');
                                    },1)
                                }else{
                                    dom.find('.active-row').removeClass('active-row');
                                    //dom.find('.comment-button').hide();
                                }
                            }
                    }
                });
            },

            [ria.mvc.DomEventBind('submit', '.grade-container form')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function submitForm(node, event){
                var res = !node.find('input[name="gradevalue"]').hasClass('error');
                if(!res)
                    return false;
                node.parent().find('.value').setHTML('...');
                return true;
            }
        ]);
});