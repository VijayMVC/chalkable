REQUIRE('chlk.lib.serialize.ChlkJsonSerializer');
REQUIRE('chlk.lib.ajax.ChlkJsonPostTask');
REQUIRE('chlk.lib.ajax.ChlkJsonGetTask');
REQUIRE('chlk.lib.ajax.UploadFileTask');
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
    var Serializer = new chlk.lib.serialize.ChlkJsonSerializer;

    /** @class chlk.services.BaseService*/
    CLASS(
        'BaseService', IMPLEMENTS(ria.mvc.IContextable), [

            ria.mvc.IContext, 'context',

            String, function getServiceRoot(){
                return this.getContext().getSession().get(ChlkSessionConstants.SITE_ROOT);
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

            Array, function arrayToIds(obj){
                return obj ? obj.map(function(item){ return item.valueOf();}) : [];
            },

            String, function arrayToCsv(obj){
                return obj ? obj.map(function(item){ return item.valueOf();}).join(',') : "";
            },

            function prepareDefaultHeaders(headers){
                var result = headers || {};
                result["X-Requested-With"] = "XMLHttpRequest";
                return result;
            },

            [[String, Object, Object]],
            ria.async.Future, function get(uri, clazz_, gParams_) {
                return new chlk.lib.ajax.ChlkJsonGetTask(this.resolveUri(uri))
                    .params(gParams_ || {})
                    .requestHeaders(this.prepareDefaultHeaders({}))
                    .run()
                    .then(function (data) {
                        if (!clazz_)
                            return data.data || null;
                        var dt = getDate().getTime();
                        var res = Serializer.deserialize(data.data, clazz_);
                        console.info('deserialize time', getDate().getTime() - dt);
                        return res;
//                        throw(new Exception(handler.getMessage()));
                    }, this);
            },

            [[String, Object, Object, Object]],
            ria.async.Future, function uploadFiles(uri, files, clazz_, gParams_) {
                return new chlk.lib.ajax.UploadFileTask(this.resolveUri(uri), files)
                    .params(gParams_ || {})
                    .requestHeaders(this.prepareDefaultHeaders({}))
                    .run()
                    .then(function (data) {
                        if (!clazz_)
                            return data.data || null;
                        return Serializer.deserialize(data.data, clazz_);
                    }, this);
            },

            [[String, Object, Object]],
            ria.async.Future, function post(uri, clazz, gParams) {
                return new chlk.lib.ajax.ChlkJsonPostTask(this.resolveUri(uri))
                    .params(gParams)
                    .requestHeaders(this.prepareDefaultHeaders({"Content-Type": "application/json; charset=utf-8"}))
                    .run()
                    .then(function (data) {
                        if (!clazz)
                            return data.data || null;
                        return Serializer.deserialize(data.data, clazz);
                    }, this);
            },

           [[String, Function]],
           function getIdsList(ids, idClass){
               var result = ids ? ids.split(',').map(function(item){
                   return new idClass(item)
               }) : [];
               return result;
           },

            [[String, String, Object]],
            ria.async.Future, function makeApiCall(uri, token, gParams) {
                return new chlk.lib.ajax.ChlkJsonPostTask(this.resolveUri(uri))
                    .params(gParams)
                    .requestHeaders(this.prepareDefaultHeaders({
                        "Content-Type": "application/json; charset=utf-8",
                        "Authorization": "Bearer:" + token
                    }))
                    .run()
                    .then(function (data) {
                        var result = {};
                        if (!data.success){
                            result = {
                                message: data.data.message
                            };
                        }
                        else{
                            result = data.data;
                        }
                        return result;
                    });
            },

            [[String, Object, Object]],
            ria.async.Future, function postArray(uri, clazz, gParams) {
                return new chlk.lib.ajax.ChlkJsonPostTask(this.resolveUri(uri))
                    .params(gParams)
                    .requestHeaders(this.prepareDefaultHeaders({"Content-Type": "application/json; charset=utf-8"}))
                    .run()
                    .then(function (data) {
                        return Serializer.deserialize(data.data, ArrayOf(clazz));
                    }, this);
            },

            [[String, Object, Object]],
            ria.async.Future, function getArray(uri, clazz, gParams) {
                return new chlk.lib.ajax.ChlkJsonGetTask(this.resolveUri(uri))
                    .params(gParams)
                    .requestHeaders(this.prepareDefaultHeaders({}))
                    .run()
                    .then(function (data) {
                        return Serializer.deserialize(data.data, ArrayOf(clazz));
//                        throw(new Exception(handler.getMessage()));
                    }, this);
            },

            [[String, Object, Object]],
            ria.async.Future, function getPaginatedList(uri, clazz, gParams) {
                return new chlk.lib.ajax.ChlkJsonGetTask(this.resolveUri(uri))
                    .params(gParams)
                    .requestHeaders(this.prepareDefaultHeaders({}))
                    .run()
                    .then(function (data) {
                        var model = new chlk.models.common.PaginatedList(clazz);
                        var dt = getDate().getTime();
                        model.setItems(Serializer.deserialize(data.data, ArrayOf(clazz)));
                        console.info('deserialize time', getDate().getTime() - dt);
                        model.setPageIndex(Number(data.pageindex));
                        model.setPageSize(Number(data.pagesize));
                        model.setActualCount(Number((data.data || []).length));
                        model.setTotalCount(Number(data.totalcount));
                        model.setTotalPages(Number(data.totalpages));
                        model.setHasNextPage(Boolean(data.hasnextpage));
                        model.setHasPreviousPage(Boolean(data.haspreviouspage));
                        return model;
                    });
//                        throw(new Exception(handler.getMessage()));
            }
        ]);
});
