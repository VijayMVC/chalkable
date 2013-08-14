REQUIRE('ria.serialize.IDeserializable');
REQUIRE('chlk.models.id.AppGradeLevelId');

NAMESPACE('chlk.models.apps', function () {
    "use strict";

    /** @class chlk.model.apps.AppGradeLevel*/
    CLASS(
        'AppGradeLevel', [
            chlk.models.id.AppGradeLevelId, 'id',
            String, 'name',
            [[chlk.models.id.AppGradeLevelId, String]],
            function $(id_, name_){
                if (id_)
                    this.setId(id_);
                if (name_)
                    this.setName(name_);
            },
        ]);
});