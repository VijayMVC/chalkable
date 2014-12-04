REQUIRE('ria.serialize.JsonSerializer');
REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.lib.serialize', function () {
    "use strict";

    /** @class chlk.lib.serialize.ChlkJsonSerializer*/
    CLASS('ChlkJsonSerializer', EXTENDS(ria.serialize.JsonSerializer), [

        OVERRIDE, Object, function deserialize(raw, clazz, instance_) {
            if (ria.__API.isClassConstructor(clazz)) {
                if (raw === null || raw === undefined || raw == '' && chlk.models.common.ChlkDate == clazz)
                    return null;
            }
            return BASE(raw, clazz, instance_);
        },

        OVERRIDE, ria.async.Future, function deserializeAsync(raw, clazz, instance_) {
            if (ria.__API.isClassConstructor(clazz)) {
                if (raw === null || raw === undefined || raw == '' && chlk.models.common.ChlkDate == clazz)
                    return null;
            }
            return BASE(raw, clazz, instance_);
        }
    ])
});