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
                    state.setController(node.find('.controller-name').getAttr('value'));
                    state.setAction(node.find('.action-name').getAttr('value'));
                    state.setParams([node.find('.page-value').getAttr('value')]);
                    state.setPublic(false);

                    this.context.stateUpdated();
                }catch(e){
                    console.info(e.getMessage());
                }

                return false;
            },

            Object, function preparePaginationData(data) {
                var start = data.pageindex*data.pagesize + 1;
                return {
                    hasPreviousPage: data.haspreviouspage,
                    hasNextPage: data.hasnextpage,
                    lastPageIndex: data.totalpages - 1,
                    prevPageIndex: data.pageindex - 1,
                    nextPageIndex: data.pageindex + 1,
                    totalCount: data.totalcount,
                    start: start,
                    end: start + data.pagesize - 1,
                    pageSize: 10
                }
            }
        ]);
});