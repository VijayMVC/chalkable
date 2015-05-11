REQUIRE('chlk.models.common.HoverBoxItem');
REQUIRE('ria.serialize.SJX');

NAMESPACE('chlk.models.common', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.common.HoverBox*/
    CLASS(
        GENERIC('THoverItem', ImplementerOf(ria.serialize.IDeserializable)),
        UNSAFE, 'HoverBox', IMPLEMENTS(ria.serialize.IDeserializable), [

            VOID, function deserialize(raw){
                this.title = SJX.fromValue(raw.title, String);
                this.name = SJX.fromValue(raw.name, String);
                this.passing = SJX.fromValue(raw.ispassing, Boolean);
                this.hover = SJX.fromArrayOfDeserializables(raw.hover, this.getSpecsOf('THoverItem'));
            },
            String, 'title',
            Boolean, 'passing',
            String, 'name',
            ArrayOf(THoverItem), 'hover'

        ]);
});
