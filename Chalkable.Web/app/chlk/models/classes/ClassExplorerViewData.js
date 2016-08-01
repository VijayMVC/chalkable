REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.standard.StandardForClassExplorer');
REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.models.classes', function(){
    "use strict";

    /**@class chlk.models.classes.ClassExplorerViewData*/

    var SJX = ria.serialize.SJX;

    CLASS(
        UNSAFE, 'ClassExplorerViewData',
                EXTENDS(chlk.models.classes.BaseClassProfileViewData),
                IMPLEMENTS(ria.serialize.IDeserializable), [

            VOID, function deserialize(raw){
                this.id = SJX.fromValue(raw.id, chlk.models.id.ClassId);
                this.teachersIds = SJX.fromArrayOfValues(raw.teachersids, chlk.models.id.SchoolPersonId);
                this.standards = SJX.fromArrayOfDeserializables(raw.standards, chlk.models.standard.StandardForClassExplorer);
                this.name = SJX.fromValue(raw.name, String);
            },

            ArrayOf(chlk.models.standard.StandardForClassExplorer), 'standards',
            chlk.models.id.ClassId, 'id',
            String, 'name',
            ArrayOf(chlk.models.id.SchoolPersonId), 'teachersIds',

            OVERRIDE, Object, function getClazz(){
                return this;
            }


    ]);

});