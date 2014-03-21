REQUIRE('chlk.templates.grading.GradingClassStandardsGridTpl');
REQUIRE('chlk.activities.common.InfoByMpPage');

NAMESPACE('chlk.activities.grading', function () {

    /** @class chlk.activities.grading.GradingClassStandardsGridPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.grading.GradingClassStandardsGridTpl)],
        'GradingClassStandardsGridPage', EXTENDS(chlk.activities.common.InfoByMpPage), [
            ArrayOf(String), 'allScores',

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
                    allScores.push(item.getName());
                    allScores.push(item.getName() + ' (fill all)');
                });
                this.setAllScores(allScores);
            },

            function addEvents(){
                new ria.dom.Dom().on('click.grade', ria.dom.DomEventHandler(this.onDocClick_));
            },

            [[Object, ria.dom.Event]],
            function onDocClick_(doc, event){
                var dom = this.dom;
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
            }
        ]);
});