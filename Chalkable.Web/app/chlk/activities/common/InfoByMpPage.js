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
                        this.closeBlock(parent);
                    }else{
                        var item = this.dom.find('.marking-period-container.open');
                        jQuery(item.find('.mp-data').valueOf()).animate({
                            height: 0
                        }, 500);
                        this.closeBlock(item);
                        parent.addClass('open');
                    }
                }
            },

            function closeBlock(node){
                setTimeout(function(){
                    node.removeClass('open');
                }, 500);
            }
        ]);
});