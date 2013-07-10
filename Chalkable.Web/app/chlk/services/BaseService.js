REQUIRE('ria.serialize.JsonSerializer');

REQUIRE('ria.ajax.JsonGetTask');

REQUIRE('chlk.models.common.PaginatedList');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.DataException */
    EXCEPTION(
        'DataException', [
            function $(msg, inner_) {
                BASE(msg, inner_);
            }
        ]);

    // Single instance
    var Serializer = new ria.serialize.JsonSerializer;

    /** @class chlk.services.BaseService*/
    CLASS(
        'BaseService', [


            String, function getServiceRoot(){
                return ria.__CFG["#require"].serviceRoot;
            },

            [[String]],
            String, function resolveUri(uri){
                return this.getServiceRoot() + uri;
            },

            [[String, Object, Object]],
            ria.async.Future, function get(uri, clazz, gParams) {

                return new ria.ajax.JsonGetTask(this.resolveUri(uri))
                    .params(gParams)
                    .run()
                    .then(function (data) {
                        if(!data.success)
                            throw chlk.services.DataException('Server error', Error(data.message));

                        return Serializer.deserialize(data.data, clazz);
                    });
            },

            [[String, Object, Object]],
            ria.async.Future, function getPaginatedList(uri, clazz, gParams) {
                return new ria.ajax.JsonGetTask(this.resolveUri(uri))
                    .params(gParams)
                    .run()
                    .then(function (data) {
                        var model = new chlk.models.common.PaginatedList(clazz);
                        model.setItems(Serializer.deserialize(data.data, ArrayOf(clazz)));
                        model.setPageIndex(Number(data.pageindex));
                        model.setPageSize(Number(data.pagesize));
                        model.setTotalCount(Number(data.totalcount));
                        model.setTotalPages(Number(data.totalpages));
                        model.setHasNextPage(Boolean(data.hasnextpage));
                        model.setHasPreviousPage(Boolean(data.haspreviousPage));

                        return model;
                    });
            }
        ]);
});
