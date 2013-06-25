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
            [[String, Object]],
            ria.async.Future, function get(uri, clazz) {
                return new ria.ajax.JsonGetTask(uri)
                    .run()
                    .then(function (data) {
                        return Serializer.deserialize(data, clazz);
                    });
            },

            [[String, Object, Number]],
            ria.async.Future, function getPaginatedList(uri, clazz, pageIndex) {
                return new ria.ajax.JsonGetTask(uri)
                    .run()
                    .then(function (data) {
                        var model = new chlk.models.common.PaginatedList(clazz);
                        model.setItems(Serializer.deserialize(data.items, ArrayOf(clazz)));
                        model.setPage(Number(data.page));
                        model.setPageSize(Number(data.pageSize));
                        model.setCount(Number(data.count));

                        return model;
                    });
            }
        ]);
});
