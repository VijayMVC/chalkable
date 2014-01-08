REQUIRE('chlk.models.common.PaginatedList');
REQUIRE('chlk.models.funds.FundsHistory');

REQUIRE('chlk.templates.ChlkTemplate');

NAMESPACE('chlk.templates.funds', function () {
    "use strict"
    /** @class chlk.templates.funds.FundsHistoryTpl*/

    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/funds/FundsHistory.jade')],
        [ria.templates.ModelBind(chlk.models.common.PaginatedList)],
        'FundsHistoryTpl', EXTENDS(chlk.templates.PaginatedList), [

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.funds.FundsHistory), 'items',

            [[Number]],
            String, function convertNumberToMoney(value){
                var res = value > 0 ? '+' : '-';
                return res + '$' + Math.abs(value);
            }
        ]);
});