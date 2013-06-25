REQUIRE('chlk.controls.Base');

REQUIRE('chlk.controls.ActionLinkControl');

NAMESPACE('chlk.controls', function () {

    /** @class chlk.controls.PaginatorControl */
    CLASS(
        'PaginatorControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/paginator.jade')(this);
            },

            [ria.mvc.DomEventBind('click', '.paginator-container .first-page:not(.disabled)')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function onFirstPageClick(node, event) {
                console.info('onFirstPageClick', node, event);
                return false;
            },

            [ria.mvc.DomEventBind('click', '.paginator-container .last-page:not(.disabled)')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function onLastPageClick(node, event) {
                console.info('onLastPageClick', node, event);
                return false;
            },

            [ria.mvc.DomEventBind('click', '.paginator-container .next-page:not(.disabled)')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function onNextPageClick(node, event) {
                console.info('onNextPageClick', node, event);
                return false;
            },

            [ria.mvc.DomEventBind('click', '.paginator-container .prev-page:not(.disabled)')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function onPrevPageClick(node, event) {
                console.info('onPrevPageClick', node, event);
                return false;
            },

            Object, function preparePaginationData(data) {
                return {
                    hasFirstLink: true,
                    hasLastLink: true,
                    lastPageIndex: 5,
                    pageSize: 10
                }
            }
        ]);
});