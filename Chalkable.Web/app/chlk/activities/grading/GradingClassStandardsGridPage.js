REQUIRE('chlk.templates.grading.GradingClassStandardsGridTpl');
REQUIRE('chlk.activities.common.InfoByMpPage');

NAMESPACE('chlk.activities.grading', function () {

    /** @class chlk.activities.grading.GradingClassStandardsGridPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.grading.GradingClassStandardsGridTpl)],
        'GradingClassStandardsGridPage', EXTENDS(chlk.activities.common.InfoByMpPage), [
            Array, 'allScores',

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
                setTimeout(function(node, event){
                    var isDown = event.keyCode == ria.dom.Keys.DOWN.valueOf();
                    var isUp = event.keyCode == ria.dom.Keys.UP.valueOf();
                    var list = this.dom.find('.autocomplete-list:visible');
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
            VOID, function gradeKeyUp(node, event){
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
                    if(event.keyCode == ria.dom.Keys.ENTER.valueOf() && !node.hasClass('error')){
                        if(list.exists()){
                            if(list.find('.see-all').hasClass('hovered')){
                                list.find('.see-all').trigger('click');
                                return false;
                            }
                            var hovered = list.find('hovered');
                            node.setValue(hovered.getHTML());
                            node.parent('form').find('input[name=gradeid]').setValue(hovered.getData('id'));
                        }
                        this.setValue(cell);
                    }else{
                        var text = node.getValue() ? node.getValue().trim() : '';
                        suggestions = text  ? this.getSuggestedValues(text) : [];
                        if(!suggestions.length)
                            node.addClass('error');
                        else
                            node.removeClass('error');
                        this.updateDropDown(suggestions, node);
                    }
                    this.updateDropDown(suggestions, node);
                }
            },

            function setValue(node, isComment_){
                var activeCell = node;
                node.find('.value').setHTML(node.find('.grade-autocomplete').getValue());
                activeCell.find('form').trigger('submit');
                var nextCell = activeCell.next().find('.edit-cell');
                if(nextCell.exists())
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
                        && !node.isOrInside('.autocomplete-list')){
                            dom.find('.autocomplete-list').setHTML('').hide();
                            if(!node.hasClass('comment-button')){
                                var parent = node.parent('.grade-value');
                                dom.find('.active-cell').removeClass('active-cell');
                                if(parent.exists()){
                                    if(!parent.hasClass('active-row')){
                                        var index = parent.getAttr('row-index');
                                        dom.find('.active-row').removeClass('active-row');
                                        dom.find('[row-index=' + index + ']').addClass('active-row');
                                    }
                                    parent.addClass('active-cell');
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
            }
        ]);
});