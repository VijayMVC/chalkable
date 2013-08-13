REQUIRE('ria.serialize.IDeserializable');
REQUIRE('chlk.models.id.AppPermissionId');

NAMESPACE('chlk.models.apps', function () {
    "use strict";

    /** @class chlk.model.apps.AppPermission*/
    CLASS(
        'AppPermission', IMPLEMENTS(ria.serialize.IDeserializable),  [
            chlk.models.id.AppPermissionId, 'id',
            Number, 'type',
            String, 'name',
            [[chlk.models.id.AppPermissionId, String, Number]],
            function $(id_, name_, type_){
                if (id_)
                    this.setId(id_);
                if (name_)
                    this.setName(name_);
                if (type_)
                    this.setType(type_);
            },


            VOID, function deserialize(raw){
                this.setId(new chlk.models.id.AppPermissionId(raw.id));
                this.setType(Number(raw.type));
                this.setName(raw.name);
            }
        ]);
});