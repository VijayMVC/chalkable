NAMESPACE('chlk.models.api', function () {
    "use strict";

    /** @class chlk.models.api.ApiParamType*/
    ENUM(
        'ApiParamType',{
            UNDEFINED: 0,
            INTEGER: 1,
            STRING: 2,
            BOOLEAN: 3,
            INTLIST: 4,
            GUID: 5,
            GUIDLIST: 6,
            LISTOFSTRINGLIST: 7,
            DATE: 8
        });
});
