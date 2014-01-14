REQUIRE('chlk.models.funds.FundsHistory');
REQUIRE('chlk.models.funds.AddCreditCardModel');
REQUIRE('chlk.lib.serialize.ChlkJsonSerializer');

NAMESPACE('chlk.models.funds', function(){
    "use strict";
    /**@class chlk.models.funds.SchoolPersonFundsViewData*/

    CLASS('SchoolPersonFundsViewData', [
        [ria.serialize.SerializeProperty('fundshistory')],
        Object, 'paginatedHistory',

        [[Object]],
        VOID, function setPaginatedHistory(history){
            this.paginatedHistory = history;
            var model = new chlk.models.common.PaginatedList(chlk.models.funds.FundsHistory);
            var serializer = new chlk.lib.serialize.ChlkJsonSerializer();
            model.setItems(serializer.deserialize(history.items, ArrayOf(chlk.models.funds.FundsHistory)));
            model.setPageIndex(Number(history.pageindex));
            model.setPageSize(Number(history.pagesize));
            model.setTotalCount(Number(history.totalcount));
            model.setTotalPages(Number(history.totalpages));
            model.setHasNextPage(Boolean(history.hasnextpage));
            model.setHasPreviousPage(Boolean(history.haspreviouspage));
            this.fundsHistory = model;
        },

        READONLY, chlk.models.common.PaginatedList, 'fundsHistory',


        chlk.models.funds.AddCreditCardModel, 'addCreditCardData',

        [ria.serialize.SerializeProperty('currentbalance')],
        Number, 'currentBalance',

        Boolean, 'transactionSuccess'
    ]);
});