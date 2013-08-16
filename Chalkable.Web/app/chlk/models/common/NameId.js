NAMESPACE('chlk.models.common', function () {
    "use strict";
    /** @class chlk.models.common.NameId*/
    CLASS(
        'NameId', [
            Number, 'id',
            String, 'name',

            [[Number, String]],
            function $(id_, name_){
                if (id_)
                    this.setId(id_);
                if (name_)
                    this.setName(name_);
            }
        ]);
});
