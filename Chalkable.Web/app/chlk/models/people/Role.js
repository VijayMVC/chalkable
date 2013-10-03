REQUIRE('chlk.models.id.SchoolPersonId');
NAMESPACE('chlk.models.people', function () {
    "use strict";


    //todo: join with chlk.models.common.role
    /** @class chlk.models.people.Role*/
    CLASS(
        'Role',  [
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
