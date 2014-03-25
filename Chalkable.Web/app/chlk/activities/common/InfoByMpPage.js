REQUIRE('chlk.activities.lib.TemplatePage');

NAMESPACE('chlk.activities.common', function () {

    /** @class chlk.activities.common.InfoByMpPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        'InfoByMpPage', EXTENDS(chlk.activities.lib.TemplatePage), [
            [ria.mvc.DomEventBind('click', '.mp-title')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function collapseClick(node, event){
                var nodeT = new ria.dom.Dom(event.target);
                if(!nodeT.hasClass('comment-button')){
                    var parent = node.parent('.marking-period-container');

                    var mpData = parent.find('.mp-data');
                    var container = mpData.find('.ann-types-container');
                    jQuery(mpData.valueOf()).animate({
                        height: parent.hasClass('open') ? 0 : (container.height() + parseInt(container.getCss('margin-bottom'), 10))
                    }, 500);

                    if(parent.hasClass('open')){
                        setTimeout(function(){
                            parent.removeClass('open');
                        }, 500);
                    }else{
                        parent.addClass('open');
                    }
                }
            }
        ]);
});