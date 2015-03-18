REQUIRE('chlk.lib.serialize.ChlkJsonSerializer');
REQUIRE('chlk.lib.ajax.ChlkJsonPostTask');
REQUIRE('chlk.lib.ajax.ChlkJsonGetTask');
REQUIRE('chlk.lib.ajax.UploadFileTask');
REQUIRE('chlk.models.common.PaginatedList');

REQUIRE('chlk.lib.exception.DataException');
REQUIRE('chlk.lib.exception.ChalkableException');
REQUIRE('chlk.lib.exception.ChalkableSisException');
REQUIRE('chlk.lib.exception.NoClassAnnouncementTypeException');

NAMESPACE('chlk.services', function () {
    "use strict";

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

            function getResponseProcessor_(clazz_) {
                return function (response) {
                    var res, dt;

                    if (response.success != true) {
                        var exceptionType = response.data.exceptiontype;
                        switch (exceptionType) {
                            case 'ChalkableSisException':
                                throw chlk.lib.exception.ChalkableSisException(response.data.message);
                            case 'ChalkableException':
                                throw chlk.lib.exception.ChalkableException(response.data.message);
                            case 'NoAnnouncementException':
                                throw chlk.lib.exception.NoAnnouncementException();
                            case 'NoClassAnnouncementTypeException':
                                throw chlk.lib.exception.NoClassAnnouncementTypeException();
                            default:
                                _DEBUG && console.error(exceptionType, response.data.message, response.stacktrace);
                                throw chlk.lib.exception.DataException(exceptionType + ': ' + response.data.message);
                        }
                    }

                    if (!clazz_)
                        return response.data || null;

                    dt = getDate().getTime();
                    res = Serializer.deserialize(response.data, clazz_);
                    _DEBUG && console.info('deserialize time', getDate().getTime() - dt);
                    return res;
                }
            },

            [[String, Object, Object, Boolean]],
            ria.async.Future, function get(uri, clazz_, gParams_, async_) {
                return new chlk.lib.ajax.ChlkJsonGetTask(this.resolveUri(uri))
                    .params(gParams_ || {})
                    .requestHeaders(this.prepareDefaultHeaders({}))
                    .disableCache()
                    .run()
                    .then(this.getResponseProcessor_(clazz_));
            },

            [[String, Object, Object, Object]],
            ria.async.Future, function uploadFiles(uri, files, clazz_, gParams_) {
                return new chlk.lib.ajax.UploadFileTask(this.resolveUri(uri), files)
                    .params(gParams_ || {})
                    .requestHeaders(this.prepareDefaultHeaders({}))
                    .run()
                    .then(this.getResponseProcessor_(clazz_));
            },

            [[String, Object, Object]],
            ria.async.Future, function post(uri, clazz, gParams) {
                return new chlk.lib.ajax.ChlkJsonPostTask(this.resolveUri(uri))
                    .params(gParams)
                    .requestHeaders(this.prepareDefaultHeaders({"Content-Type": "application/json; charset=utf-8"}))
                    .run()
                    .then(this.getResponseProcessor_(clazz));
            },

           [[String, Function]],
           function getIdsList(ids, idClass){
               var result = ids ? ids.split(',').map(function(item){
                   return new idClass(item)
               }) : [];
               return result;
           },

            [[String, String, Object, String]],
            ria.async.Future, function makeApiCall(uri, token, gParams, requestMethod) {
                var taskType = requestMethod == "Get" ? chlk.lib.ajax.ChlkJsonGetTask : chlk.lib.ajax.ChlkJsonPostTask;
                return new taskType(this.resolveUri(uri))
                    .params(gParams)
                    .requestHeaders(this.prepareDefaultHeaders({
                        "Content-Type": "application/json; charset=utf-8",
                        "Authorization": "Bearer:" + token
                    }))
                    .run();
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
                    .disableCache()
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
                    .disableCache()
                    .run()
                    .then(function (data) {
                        var model = new chlk.models.common.PaginatedList(clazz);
                        var dt = getDate().getTime();
                        model.setItems(Serializer.deserialize(data.data, ArrayOf(clazz)));
                        _DEBUG && console.info('deserialize time', getDate().getTime() - dt);
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
