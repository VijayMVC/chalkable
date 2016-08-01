REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.search.SearchItem');




NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.SearchService*/
    CLASS(
        'SearchService', EXTENDS(chlk.services.BaseService), [

            [[String]],
            ria.async.Future, function search(query_) {
                return this.get('Search/Search.json', ArrayOf(chlk.models.search.SearchItem), {
                    query: query_
                });
            }
        ])
});

