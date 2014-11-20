REQUIRE('ria.serialize.SJX');
REQUIRE('ria.serialize.IDeserializable');
REQUIRE('chlk.models.id.SchoolPersonId');
NAMESPACE('chlk.models.people', function () {
    "use strict";

    var roleCache = {};
    var SJX = ria.serialize.SJX;

    //todo: join with chlk.models.common.role
    /** @class chlk.models.people.Role*/
    CLASS(
        FINAL, UNSAFE, 'Role', IMPLEMENTS(ria.serialize.IDeserializable),  [
            
            VOID, function deserialize(raw){
                this.description = SJX.fromValue(raw.description, String);
                this.id = SJX.fromValue(raw.id, Number);
                this.name = SJX.fromValue(raw.name, String);
                this.nameLowered = SJX.fromValue(raw.namelowered, String);
            },

            String, 'description',
            Number, 'id',
            String, 'name',
            String, 'nameLowered',

            function $$(instance, Clazz, ctor, args) {
                if (ctor == SELF.prototype.$create) {
                    var roleId = args[0];
                    if (roleCache.hasOwnProperty(roleId))
                        return roleCache[roleId];

                    return roleCache[roleId] = new ria.__API.init(instance, Clazz, ctor, args);
                }
                return new ria.__API.init(instance, Clazz, ctor, args);
            },

            [[Number, String, String, String]],
            function $create(id, name, nameLowered, description){
                BASE();
                this.id = id;
                this.name = name;
                this.namelowered = nameLowered;
                this.description = description;
            }
    ]);
});
