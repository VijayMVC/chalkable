REQUIRE('ria.serialize.JsonSerializer');

REQUIRE('ria.ajax.JsonGetTask');
REQUIRE('ria.ajax.JsonPostTask');
REQUIRE('ria.ajax.UploadFileTask');

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


                var siteRoot = ria.__CFG["#require"].siteRoot;
                var serviceRoot = ria.__CFG["#require"].serviceRoot;

                if (siteRoot[siteRoot.length - 1] != '/' && serviceRoot[0] != '/'){
                    siteRoot += '/';
                }

                return siteRoot + serviceRoot;
            },

            [[String]],
            String, function resolveUri(uri){
                return this.getServiceRoot() + uri;
            },

            String, function getUrl(uri, params){
                var p = params, r = [];
                for(var key in p) if (p.hasOwnProperty(key)) {
                    r.push([key, p[key]].join('='));
                }
                return this.resolveUri(uri) + '?' +  r.join('&');
            },

            [[String, Object, Object]],
            ria.async.Future, function get(uri, clazz_, gParams_) {

                return new ria.ajax.JsonGetTask(this.resolveUri(uri))
                    .params(gParams_ || {})
                    .run()
                    .then(function (data) {
                        if(!data.success)
                            throw chlk.services.DataException('Server error', Error(data.message));

                        if (!clazz_)
                            return data.data || null;

                        return Serializer.deserialize(data.data, clazz_);
                    });
            },

            [[String, Object, Object, Object]],
            ria.async.Future, function uploadFiles(uri, files, clazz_, gParams_) {

                return new ria.ajax.UploadFileTask(this.resolveUri(uri), files)
                    .params(gParams_ || {})
                    .run()
                    .then(function (data) {
                        if(!data.success)
                            throw chlk.services.DataException('Server error', Error(data.message));

                        if (!clazz_)
                            return data.data || null;

                        return Serializer.deserialize(data.data, clazz_);
                    });
            },


            [[String, Object, Object]],
            ria.async.Future, function post(uri, clazz, gParams) {

                return new ria.ajax.JsonPostTask(this.resolveUri(uri))
                    .params(gParams)
                    .run()
                    .then(function (data) {
                        if(!data.success)
                            throw chlk.services.DataException('Server error', Error(data.message));

                        return Serializer.deserialize(data.data, clazz);
                    });
            },

            [[String, Object, Object]],
            ria.async.Future, function postArray(uri, clazz, gParams) {

                return new ria.ajax.JsonPostTask(this.resolveUri(uri))
                    .params(gParams)
                    .run()
                    .then(function (data) {

                        if(!data.success)
                            throw chlk.services.DataException('Server error', Error(data.message));

                        return Serializer.deserialize(data.data, ArrayOf(clazz));
                    });
            },

            [[String, Object, Object]],
            ria.async.Future, function getArray(uri, clazz, gParams) {

                return new ria.ajax.JsonGetTask(this.resolveUri(uri))
                    .params(gParams)
                    .run()
                    .then(function (data) {

                        if(!data.success)
                            throw chlk.services.DataException('Server error', Error(data.message));

                        return Serializer.deserialize(data.data, ArrayOf(clazz));
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
                        model.setHasPreviousPage(Boolean(data.haspreviouspage));

                        return model;
                    });
            }
        ]);
});
