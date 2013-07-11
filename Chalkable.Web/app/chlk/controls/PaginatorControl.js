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
                    var configs = this.getConfigs();
                    state.setController(node.find('.controller-name').getAttr('value'));
                    state.setAction(node.find('.action-name').getValue());
                    var pageNode = node.find('.page-value');
                    var value = (parseInt(pageNode.getValue(), 10) - 1) * configs.pageSize;
                    if(value > configs.lastPageStart)
                        value = configs.lastPageStart;
                    if(value < 0)
                        value = 0;
                    pageNode.setValue(1 + value / configs.pageSize);
                    state.setParams([value]);
                    state.setPublic(false);

                    this.context.stateUpdated();
                }catch(e){
                    console.info(e.getMessage());
                }

                return false;
            },

            Object, 'configs',

            Object, function preparePaginationData(data) {
                var start = data.pageindex*data.pagesize;
                var res = {
                    hasPreviousPage: data.haspreviouspage,
                    hasNextPage: data.hasnextpage,
                    lastPageStart: (data.totalpages - 1) * data.pagesize,
                    prevPageStart: start - data.pagesize,
                    nextPageStart: start + data.pagesize,
                    pageIndex: data.pageindex + 1,
                    totalCount: data.totalcount,
                    startText: start + 1,
                    end: start + data.pagesize,
                    pageSize: data.pagesize
                };
                this.setConfigs(res);
                return res;
            }
        ]);
});