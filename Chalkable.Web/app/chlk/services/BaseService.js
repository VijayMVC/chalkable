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

            [[String, Object]],
            ria.async.Future, function get(uri, clazz) {

                return new ria.ajax.JsonGetTask(this.resolveUri(uri))
                    .run()
                    .then(function (data) {
                        if(!data.success)
                            throw chlk.services.DataException('Server error', Error(data.message));

                        return Serializer.deserialize(data.data, clazz);
                    });
            },

            [[String, Object, Number]],
            ria.async.Future, function getPaginatedList(uri, clazz, pageIndex) {
                return new ria.ajax.JsonGetTask(this.resolveUri(uri))
                    .run()
                    .then(function (data) {
                        var model = new chlk.models.common.PaginatedList(clazz);
                        model.setItems(Serializer.deserialize(data.data, ArrayOf(clazz)));
                        model.setPageindex(Number(data.pageindex));
                        model.setPagesize(Number(data.pagesize));
                        model.setTotalcount(Number(data.totalcount));
                        model.setTotalpages(Number(data.totalpages));
                        model.setHasnextpage(Boolean(data.hasnextpage));
                        model.setHaspreviouspage(Boolean(data.haspreviouspage));

                        return model;
                    });
            }
        ]);
});
