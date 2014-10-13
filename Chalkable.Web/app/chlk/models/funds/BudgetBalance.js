NAMESPACE('chlk.models.funds', function () {
    "use strict";
    /** @class chlk.models.funds.BudgetBalance*/
    CLASS(
        'BudgetBalance', [
            [ria.serialize.SerializeProperty('startschoolbalance')],
            Number, 'startSchoolBalance',

            [ria.serialize.SerializeProperty('currentbalance')],
            Number, 'currentBalance',

            [ria.serialize.SerializeProperty('persentspent')],
            Number, 'percentSpent'
        ]);
});
