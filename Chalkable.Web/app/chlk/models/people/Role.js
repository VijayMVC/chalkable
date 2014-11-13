REQUIRE('chlk.models.id.SchoolPersonId');
NAMESPACE('chlk.models.people', function () {
    "use strict";

    var roleCache = {};

    //todo: join with chlk.models.common.role
    /** @class chlk.models.people.Role*/
    CLASS(
        'Role',  [

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
            },

            String, 'description',

            Number, 'id',

//            [[Number]],
//            VOID, function setId(id){
//                this._id = id;
//                if(this.getRoleId().valueOf() != id)
//                    this.setRoleId(new chlk.models.common.RoleEnum(id));
//            },

            String, 'name',
//            [[String]],
//            VOID, function setName(name){
//                this._name = name;
//                if(this.getRoleName() != name)
//                    this.setRoleName(name);
//            },

            [ria.serialize.SerializeProperty('namelowered')],
            String, 'nameLowered'
//
//            [[chlk.models.common.RoleEnum, String]],
//            function $(roleId_, roleName_){
//                BASE(roleId_, roleName_);
//                if(roleId_)
//                    this._id = roleId_.valueOf();
//                if(roleName_){
//                    this._name = roleName_;
//                    this.setNameLowered(roleName_.toLowerCase());
//                }
//            }
    ]);
});
