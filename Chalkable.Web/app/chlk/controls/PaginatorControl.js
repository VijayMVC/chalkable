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

            [ria.mvc.DomEventBind('submit', '.paginator-container form')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function onPrevPageClick(node, event) {
                try{
                    var state = this.context.getState();
                    var controller = node.find('.controller-name').getAttr('value');
                    var action = node.find('.action-name').getAttr('value');
                    var params = [node.find('.page-value').getAttr('value')];
                    state.setController(controller);
                    state.setAction(action);
                    state.setParams(params);
                    state.setPublic(false);
                    this.context.stateUpdated();
                }catch(e){
                    console.info(e.getMessage());
                }

                return false;
            },

            Object, function preparePaginationData(data) {
                var start = data.pageIndex * data.pageSize + 1;
                return {
                    hasPreviousPage: data.hasPreviousPage,
                    hasNextPage: data.hasNextPage,
                    lastPageIndex: data.totalPages - 1,
                    prevPageIndex: data.pageIndex - 1,
                    nextPageIndex: data.pageIndex + 1,
                    totalCount: data.totalCount,
                    start: start,
                    end: start + data.pageSize - 1,
                    pageSize: 10
                }
            }
        ]);
});