REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

    /** @class app.controls.ActionLink */
    CLASS(
        'PaginatorControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/paginator.jade')(this);
            },

            [ria.mvc.DomEventBind('click', '.first-page:not(.disabled)')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function onFirstPageClick(node, event) {
                console.info('onFirstPageClick', node, event);
                return false;
            },

            [ria.mvc.DomEventBind('click', '.last-page:not(.disabled)')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function onLastPageClick(node, event) {
                console.info('onLastPageClick', node, event);
                return false;
            },

            [ria.mvc.DomEventBind('click', '.next-page:not(.disabled)')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function onNextPageClick(node, event) {
                console.info('onNextPageClick', node, event);
                return false;
            },

            [ria.mvc.DomEventBind('click', '.prev-page:not(.disabled)')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function onPrevPageClick(node, event) {
                console.info('onPrevPageClick', node, event);
                return false;
            }
        ]);
});