REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

    /** @class app.controls.Grid */
    CLASS(
        'GridControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/grid.jade')(this);
            },

            [[Array, Object]],
            VOID, function onStart(data,configs_) {
                configs_ && configs_.selectedIndex !== undefined && this.setCurrentIndex(configs_.selectedIndex);
                this.setCount(data.length);
                setTimeout(function(){
                    this.getCurrentIndex() !== undefined && this.focusGrid();
                    var grid = new ria.dom.Dom('.chlk-grid');
                    this.setGrid(grid);
                }.bind(this), 1);
            },

            Number, 'currentIndex',
            Number, 'count',
            ria.dom.Dom, 'grid',

            [ria.mvc.DomEventBind('click', '.chlk-grid .row:not(.selected)')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function rowClick(node, event) {
                var parent = node.parent('.chlk-grid');
                parent.find('.selected').removeClass('selected');
                node.addClass('selected');
                this.setCurrentIndex(parseInt(node.getAttr('index'), 10));
            },

            [ria.mvc.DomEventBind('keydown', '.grid-focus')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function keyArrowsClick(node, event) {
                var currentIndex = this.getCurrentIndex();
                if((event.which == 38 && currentIndex) || (event.which == 40 && currentIndex < this.getCount() - 1)){
                    var parent = node.parent('.chlk-grid');
                    parent.find('.selected').removeClass('selected');
                    if(event.which == 38){
                        currentIndex--;
                    }else{
                        currentIndex++;
                    }
                    parent
                        .find('.row:eq(' + currentIndex + ')')
                        .addClass('selected');
                    this.setCurrentIndex(currentIndex);
                    this.scrollToElement();
                }
            },

            [ria.mvc.DomEventBind('click', '.chlk-grid')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function onFocusGrid(node, event) {
                var target = new ria.dom.Dom(event.target);
                if(target.is(':not(input, textarea)'))
                    this.focusGrid(node);
            },

            [[ria.dom.Dom]],
            VOID, function focusGrid(node_) {
                node_ = node_ || new ria.dom.Dom('.chlk-grid');
                node_.find('.grid-focus').valueOf()[0].focus();
            },

            VOID, function scrollToElement(){
                var el = this.getGrid().find('.row.selected');
                var demo = new ria.dom.Dom('#demo-footer');
                window.scrollTo(0, el.offset().top + el.height()/2 - (window.innerHeight - (demo.height() || 0))/2);
            }
        ]);
});