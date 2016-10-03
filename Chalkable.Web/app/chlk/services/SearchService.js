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
            },

            [[ArrayOf(chlk.models.search.SearchTypeEnum), String]],
            ria.async.Future, function searchByTypes(types, query_) {
                return this.get('Search/Search.json', ArrayOf(chlk.models.search.SearchItem), {
                    includedSearchType: this.arrayToCsv(types),
                    query: query_
                });
            }
        ])
});

