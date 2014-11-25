NAMESPACE('chlk.models.standard', function () {
    "use strict";
    /** @class chlk.models.standard.CCStandardsCodesList*/
    CLASS(
        'CCStandardsCodesList', [

            ArrayOf(String), 'standardsCodes',

            [[ArrayOf(String)]],
            function $(standardsCodes){
                BASE();
                if(standardsCodes)
                    this.setStandardsCodes(standardsCodes);
            }
    ]);
});
