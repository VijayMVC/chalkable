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
                var state = this.context.getState();
                var configs = this.getConfigs();
                var actionNode = node.find('.action-name');
                var action = actionNode.getAttr('value');
                state.setAction(actionNode.getValue());
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
                return false;
            },

            Object, 'configs',

            Object, 'linkParams',

            [[Object, Array]],
            Object, function preparePaginationData(data, params_) {
                var start = data.getPageIndex() * data.getPageSize();
                this.setLinkParams(params_);
                var res = {
                    hasPreviousPage: data.isHasPreviousPage() | 0,
                    hasNextPage: data.isHasNextPage() | 0,
                    lastPageStart: ((data.getTotalPages() - 1) * data.getPageSize()) | 0,
                    prevPageStart: (start - data.getPageSize()) | 0,
                    nextPageStart: (start + data.getPageSize()) | 0,
                    pageIndex: (data.getPageIndex() + 1) | 0,
                    totalCount: data.getTotalCount() | 0,
                    totalPages: data.getTotalPages() | 0,
                    startText: (start + 1) | 0,
                    end: (start + data.getPageSize()) | 0,
                    pageSize: data.getPageSize() | 0
                };
                this.setConfigs(res);
                return res;
            }
        ]);
});