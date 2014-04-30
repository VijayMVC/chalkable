REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.attendance.SummaryPage');

NAMESPACE('chlk.activities.attendance', function () {

    var interval;

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
                var panelToShow = this.dom.find('.absent-late-page[data-page=' + page + ']');
                panelToShow.show();
                this.dom.find('.absent-late-page[data-page!=' + page + ']').hide();
                this.dom.find('.hidden-students-block').setCss('height', '0').addClass('height-0');

                clearInterval(interval);
                var grid = panelToShow.find('.students-container');
                var pageHeight = document.documentElement.clientHeight;
                var baseContentHeight = grid.height();
                var scrollPosition;
                var contentHeight = baseContentHeight + grid.offset().top;

                interval = setInterval(function(grid){
                    scrollPosition = window.pageYOffset;
                    var hiddenBlock = panelToShow.find('.hidden-students-block.height-0:eq(0)');
                    if(hiddenBlock.exists()){
                        if((contentHeight - pageHeight - scrollPosition) < 400){
                            var height = hiddenBlock.find('.hidden-students-block-2').height();
                            var div = new ria.dom.Dom('<div class="horizontal-loader"></div>');
                            grid.appendChild(div);
                            setTimeout(function(div){
                                div.remove();
                                hiddenBlock.find('.horizontal-loader').remove();
                                hiddenBlock
                                    .setCss('height', height)
                                    .removeClass('height-0');
                            }.bind(this, div), 400);
                            contentHeight += baseContentHeight;
                        }
                    }
                    else{
                        clearInterval(interval);
                    }
                }.bind(this, grid), 500);
            },

            [ria.mvc.DomEventBind('mouseleave', '.student-with-picture')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function studentHover1(node, event){
                var popUp = node.find('.chlk-pop-up-container');
                popUp.hide();
            },

            [ria.mvc.DomEventBind('mousemove', '.alerts')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function studentHover2(node, event){
                var popUp = node.parent('.student-with-picture').find('.chlk-pop-up-container');
                popUp.hide();
            },

            [ria.mvc.DomEventBind('mousemove', '.student-with-picture .avatar-wrapper>div')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function studentHover3(node, event){
                var popUp = node.parent('.student-with-picture').find('.chlk-pop-up-container');
                popUp.show();
                var left = ((node.width() - popUp.width()) / 2) - 8 + 'px';
                popUp.setCss('left', left);
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