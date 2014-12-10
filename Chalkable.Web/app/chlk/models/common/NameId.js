REQUIRE('ria.serialize.SJX');

NAMESPACE('chlk.models.common', function () {
    "use strict";

    var SJX = ria.serialize.SJX;
    /** @class chlk.models.common.NameId*/
    CLASS(

        //todo this f... class is used everywhere, should be made generic
        UNSAFE, FINAL, 'NameId', IMPLEMENTS(ria.serialize.IDeserializable), [

            VOID, function deserialize(raw){
                this.id = raw.id;//fix this
                this.name = SJX.fromValue(raw.name, String);
            },

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
