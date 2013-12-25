REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.attendance.SummaryPage');

NAMESPACE('chlk.activities.attendance', function () {

    /** @class chlk.activities.attendance.SummaryPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.attendance.SummaryPage)],
        'SummaryPage', EXTENDS(chlk.activities.lib.TemplatePage), [
            [ria.mvc.DomEventBind('click', '.absent-late-button:not(.pressed)')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function buttonClick(node, event){
                this.dom.find('.absent-late-button.pressed').removeClass('pressed');
                node.addClass('pressed');
                var page = node.getData('page');
                this.dom.find('.absent-late-page[data-page=' + page + ']').show();
                this.dom.find('.absent-late-page[data-page!=' + page + ']').hide();
                this.dom.find('.hidden-students-block').setCss('height', '0');
            }/*,

            function setScroller(){
                var grid = this.dom.find('.students-container:visible');
                var baseContentHeight = grid.height();

                var pageHeight = document.documentElement.clientHeight;
                var scrollPosition;
                var contentHeight = baseContentHeight + grid.offset().top;
                var interval;

                interval = setInterval(function(){
                    if(grid.find('.hidden-students-block:not(.processed)')){
                        if((contentHeight - pageHeight - scrollPosition) < 400){
                            configs.currentStart += configs.pageSize;
                            this.scrollAction_(grid);
                            contentHeight += baseContentHeight;
                        }
                    }
                    else{
                        clearInterval(interval);
                    }
                }.bind(this));
            },

            OVERRIDE, VOID, function onRender_(model){
                BASE(model);

            }*/
        ]);
});