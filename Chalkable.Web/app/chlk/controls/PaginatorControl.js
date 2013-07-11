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
                    var action = node.find('.action-name').getAttr('value');
                    state.setAction(node.find('.action-name').getValue());
                    var pageNode = node.find('.page-value');
                    var value = (parseInt(pageNode.getValue(), 10) - 1) * configs.pageSize;
                    if(value > configs.lastPageStart)
                        value = configs.lastPageStart;
                    if(value < 0)
                        value = 0;
                    pageNode.setValue(1 + value / configs.pageSize);
                    var params = this.getLinkParams().slice();
                    params.push(value);
                    state.setParams(params);
                    state.setPublic(false);

                    this.context.stateUpdated();
                }catch(e){
                    console.info(e.getMessage());
                }

                return false;
            },

            Object, 'configs',

            Object, 'linkParams',

            [[Object, Array]],
            Object, function preparePaginationData(data, params_) {
                var start = data.pageIndex*data.pageSize;
                this.setLinkParams(params_);
                var res = {
                    hasPreviousPage: data.hasPreviousPage,
                    hasNextPage: data.hasNextPage,
                    lastPageStart: (data.totalPages - 1) * data.pageSize,
                    prevPageStart: start - data.pageSize,
                    nextPageStart: start + data.pageSize,
                    pageIndex: data.pageIndex + 1,
                    totalCount: data.totalCount,
                    startText: start + 1,
                    end: start + data.pageSize,
                    pageSize: data.pageSize
                };
                this.setConfigs(res);
                return res;
            }
        ]);
});