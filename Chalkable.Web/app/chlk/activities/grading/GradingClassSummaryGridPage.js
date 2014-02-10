REQUIRE('chlk.templates.grading.GradingClassSummaryGridTpl');
REQUIRE('chlk.activities.common.InfoByMpPage');
REQUIRE('chlk.models.common.Array');

NAMESPACE('chlk.activities.grading', function () {

    /** @class chlk.activities.grading.GradingClassSummaryGridPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.grading.GradingClassSummaryGridTpl)],
        'GradingClassSummaryGridPage', EXTENDS(chlk.activities.common.InfoByMpPage), [
            [[String]],
            ArrayOf(String), function getSuggestedValues(text){
                var text = text.toLowerCase();
                var res1 =  {
                    'a': ['A+', 'A', 'A-', 'A (fill all)'],
                    'a+': ['A+', 'A+ (fill all)'],
                    'a-': ['A-', 'A- (fill all)'],
                    'b': ['B+', 'B', 'B-', 'B (fill all)'],
                    'b+': ['B+', 'B+ (fill all)'],
                    'b-': ['B-', 'B- (fill all)'],
                    'c': ['C+', 'C', 'C-', 'Complete', 'C (fill all)', 'Complete (fill all)'],
                    'c+': ['C+', 'C+ (fill all)'],
                    'c-': ['C-', 'C- (fill all)'],
                    'd': ['D+', 'D', 'D-', 'D (fill all)', 'Dropped', 'Dropped (fill all)'],
                    'd+': ['D+', 'D+ (fill all)'],
                    'd-': ['D-', 'D- (fill all)'],
                    'f': ['F+', 'F', 'F-',' F (fill all)'],
                    'f+': ['F+', 'F+ (fill all)'],
                    'f-': ['F-', 'F- (fill all)'],
                    'e': ['Exempt', 'Exempt (fill all)'],
                    'x': ['Exempt', 'Exempt (fill all)']
                };

                var res2 =  {
                    'complete': ['Complete', 'Complete (fill all)'],
                    'complete (fill all)': ['Complete (fill all)'],
                    'exempt': ['Exempt', 'Exempt (fill all)'],
                    'exempt (fill all)': ['Exempt (fill all)'],
                    'fail': ['Fail', 'Fail (fill all)'],
                    'fail (fill all)': ['Fail (fill all)'],
                    'incomplete': ['Incomplete', 'Incomplete (fill all)'],
                    'incomplete (fill all)': ['Incomplete (fill all)'],
                    'late': ['Late', 'Late (fill all)'],
                    'late (fill all)': ['Late (fill all)'],
                    'pass': ['Pass', 'Pass (fill all)'],
                    'pass (fill all)': ['Pass (fill all)'],
                    'dropped': ['Dropped', 'Dropped (fill all)'],
                    'dropped (fill all)': ['Dropped (fill all)']
                };

                if(res1[text])
                    return res1[text];
                var res = [];
                if(text.length > 1)
                    for(var p in res2)
                        if(res2.hasOwnProperty(p))
                            if(p.indexOf(text) == 0 && (res.length < res2[p].length))
                                res = res2[p];
                return res;
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

            VOID, function hideDropDown(){
                var list = this.dom.find('.autocomplete-list');
                list.setHTML('')
                    .hide();
            },

            [ria.mvc.DomEventBind('keyup', '.grade-autocomplete')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function gradeKeyUp(node, event){
                var suggestions = [], cell = node.parent('.active-cell');
                var isDown = event.keyCode == ria.dom.Keys.DOWN.valueOf();
                var isUp = event.keyCode == ria.dom.Keys.UP.valueOf();
                var list = this.dom.find('.autocomplete-list:visible');
                if(isDown || isUp){
                    if(list.exists()){
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
                }else{
                    if(event.keyCode == ria.dom.Keys.ENTER.valueOf() && !node.hasClass('error')){
                        if(list.exists() && list.find('.see-all').hasClass('hovered')){
                            list.find('.see-all').trigger('click');
                            return false;
                        }
                        else
                            this.setValue(cell);
                    }else{
                        var text = node.getValue() ? node.getValue().trim() : '';
                        var parsed = parseInt(text,10);
                        if(parsed){
                            node.removeClass('error');
                            if(parsed != text){
                                /*var len = parsed.toString().length;
                                if(text[len] == ' '){
                                    var comment = text.slice(len);
                                    cell.find('.comment-value').setValue(comment);
                                    node.parent('.marking-period-container').find('.comment-button').trigger('click');
                                }else{
                                    node.addClass('error');
                                }*/
                                node.addClass('error');
                            }else{
                                this.hideDropDown();
                            }
                        }else{
                            suggestions = text  ? this.getSuggestedValues(text) : [];
                            if(!suggestions.length)
                                node.addClass('error');
                            else
                                node.removeClass('error');
                            this.updateDropDown(suggestions, node);
                        }
                    }
                    this.updateDropDown(suggestions, node);
                }
            },

            [ria.mvc.DomEventBind('click', '.see-all')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function seeAllClick(node, event){
                var res = ['A+', 'A', 'A-','B+', 'B', 'B-','C+', 'C', 'C-','D+', 'D', 'D-','F',
                    'F (fill all)','A (fill all)','B (fill all)','C (fill all)','D (fill all)',
                    'Complete', 'Complete (fill all)', 'Dropped', 'Dropped (fill all)', 'Exempt',
                    'Exempt (fill all)', 'Fail', 'Fail (fill all)', 'Incomplete',
                    'Incomplete (fill all)', 'Late', 'Late (fill all)', 'Pass', 'Pass (fill all)'];
                this.updateDropDown(res, this.dom.find('.active-cell'), true);
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
                var popUp = this.dom.find('.chlk-pop-up-container.comment');
                var main = this.dom.parent('#main');
                var bottom = main.height() + main.offset().top - active.offset().top + 73;
                var left = active.offset().left - main.offset().left - 54;
                popUp.setCss('bottom', bottom);
                popUp.setCss('left', left);
                popUp.find('textarea').setValue(active.find('.comment-value').getValue());
                popUp.show();
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

            [ria.mvc.DomEventBind('click', '.autocomplete-item:not(.see-all)')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function listItemBtnClick(node, event){
                var cell = this.dom.find('.active-cell');
                cell.find('.error').removeClass('error');
                this.setValue(cell);
                return false;
            },

            function setValue(node, isComment_){
                var nextCell = this.dom.find('.active-cell').next().find('.edit-cell');
                if(!isComment_){
                    if(nextCell.exists())
                        nextCell.trigger('click');
                    else
                        this.dom.trigger('click');
                }
            },

            /*function getNormalValue(text){
                var mapping = this.getGradingStyleMapper();
                var gradingStyle = 1;

                var value = GradingStyler.getGradeNumberValue(text, mapping, gradingStyle);
            },

            chlk.models.grading.Mapping, 'gradingStyleMapper',*/

            OVERRIDE, VOID, function onRender_(model){
                BASE(model);
                //this.setGradingStyleMapper(model.getGradingStyleMapper());
                var dom = this.dom;
                new ria.dom.Dom().on('click.grade', function(doc, event){
                    var popUp = dom.find('.chlk-pop-up-container.comment');
                    var node = new ria.dom.Dom(event.target);
                    if(!node.hasClass('grade-autocomplete') && !node.hasClass('arrow')
                        && !node.isOrInside('.chlk-pop-up-container.comment') && !node.isOrInside('.autocomplete-list')){
                            dom.find('.autocomplete-list').setHTML('').hide();
                            if(!node.hasClass('comment-button')){
                                popUp.hide();
                                var parent = node.parent('.grade-value');
                                dom.find('.active-cell').removeClass('active-cell');
                                if(parent.exists() && !node.hasClass('alert-flag')){
                                    if(!parent.hasClass('active-row')){
                                        var index = parent.getAttr('row-index');
                                        dom.find('.active-row').removeClass('active-row');
                                        dom.find('[row-index=' + index + ']').addClass('active-row');
                                    }
                                    parent.addClass('active-cell');
                                    node.parent('.marking-period-container').find('.comment-button').show();
                                    setTimeout(function(){
                                        parent.find('input').trigger('focus');
                                    },1)
                                }else{
                                    dom.find('.active-row').removeClass('active-row');
                                    dom.find('.comment-button').hide();
                                }
                            }
                    }
                });
            },

            OVERRIDE, VOID, function onStop_() {
                BASE();
                new ria.dom.Dom().off('click.grade');
            }
        ]);
});