REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.common.NameId');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.GradeLevelService*/
    CLASS(
        'GradeLevelService', EXTENDS(chlk.services.BaseService), [

            Array, function getGradeLevels() {
                 return new ria.serialize.JsonSerializer().deserialize([
                     {name: '1st', id: 1},
                     {name: '2st', id: 2},
                     {name: '3st', id: 3},
                     {name: '4st', id: 4},
                     {name: '5st', id: 5},
                     {name: '6st', id: 6},
                     {name: '7st', id: 7},
                     {name: '8st', id: 8},
                     {name: '9st', id: 9},
                     {name: '10st', id: 10},
                     {name: '11st', id: 11},
                     {name: '12st', id: 12}
                 ], ArrayOf(chlk.models.common.NameId));
            },
        ])
});