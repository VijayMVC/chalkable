NAMESPACE('chlk.models.common', function () {
    "use strict";

    /** @class chlk.models.common.Logout*/
    CLASS(
        'Logout', [
            String, 'name',

            function $(name){
                this.setName(name);
            }

        ]);
});
