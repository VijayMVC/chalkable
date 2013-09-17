NAMESPACE('chlk.models.common', function () {
    "use strict";
    /** @class chlk.models.common.NameId*/
    CLASS(
        'NameId', [
            Object, 'id',
            String, 'name',

            [[Object, String]],
            function $(id_, name_){
                BASE();
                if (id_)
                    this.setId(id_);
                if (name_)
                    this.setName(name_);
            }
        ]);
});
