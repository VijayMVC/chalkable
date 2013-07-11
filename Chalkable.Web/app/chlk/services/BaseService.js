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
            [[String, Object, Object]],
            ria.async.Future, function get(uri, clazz, params_) {
                return new ria.ajax.JsonGetTask(this.getUrl(uri, params_))
                    .run()
                    .then(function (data) {
                        if(!data.success)
                            throw chlk.services.DataException('Server error', Error(data.message));

                        return Serializer.deserialize(data.data, clazz);
                    });
            },

            [[String, Object]],
            String, function getUrl(uri, params_){
                if(!params_)
                    return uri;
                if(uri[uri.length - 1] != '?')
                    uri+='?'
                var arr=[];
                for(var param in params_){
                    if(params_.hasOwnProperty(param))
                        arr.push(param + '=' + params_[param])
                }
                return uri + arr.join('&');
            },

            [[String, Object, Object]],
            ria.async.Future, function getPaginatedList(uri, clazz, params_) {
                return new ria.ajax.JsonGetTask(this.getUrl(uri, params_))
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
