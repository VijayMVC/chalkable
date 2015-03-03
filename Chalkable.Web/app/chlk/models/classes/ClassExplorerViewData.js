REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.standard.StandardForClassExplorer');
REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.models.classes', function(){
    "use strict";

    /**@class chlk.models.classes.ClassExplorerViewData*/

    var SJX = ria.serialize.SJX;

    CLASS(
        UNSAFE, 'ClassExplorerViewData', EXTENDS(chlk.models.classes.BaseClassProfileViewData), [

            ArrayOf(chlk.models.standard.StandardForClassExplorer), 'standards',

            chlk.models.id.ClassId, 'id',

            OVERRIDE, Object, function getClazz(){
                return this;
            },

            VOID, function deserialize(raw){
                this.id = SJX.fromValue(raw.id, chlk.models.id.ClassId);
                this.standards = SJX.fromArrayOfDeserializables(raw.standards, chlk.models.standard.StandardForClassExplorer);
            }
    ]);

});