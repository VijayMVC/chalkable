REQUIRE('chlk.lib.serialize.ChlkJsonSerializer');
REQUIRE('chlk.lib.ajax.ChlkJsonPostTask');
REQUIRE('chlk.lib.ajax.ChlkJsonGetTask');
REQUIRE('chlk.lib.ajax.UploadFileTask');
REQUIRE('chlk.models.common.PaginatedList');

REQUIRE('chlk.lib.exception.DataException');
REQUIRE('chlk.lib.exception.ChalkableException');
REQUIRE('chlk.lib.exception.ChalkableSisException');
REQUIRE('chlk.lib.exception.NoAnnouncementException');
REQUIRE('chlk.lib.exception.ChalkableApiException');
REQUIRE('chlk.lib.exception.NoClassAnnouncementTypeException');
REQUIRE('chlk.lib.exception.ChalkableSisNotSupportVersionException');
REQUIRE('chlk.lib.exception.FileSizeExceedException');
REQUIRE('chlk.lib.exception.AnnouncementDeleteFailedException');


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
                var queryString = this.buildQueryString_(params);
                return this.resolveUri(uri) + '?' +  queryString;
            },

            String, function getApiUrl(url, params){
                var queryString = this.buildQueryString_(params);
                return url + '?' + queryString;
            },

            String, function buildQueryString_(params){
                var p = params, r = [];
                for(var key in p) if (p.hasOwnProperty(key)) {
                    r.push([key, p[key]].join('='));
                }
                return r.join('&');
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
                var baseService = this;
                return function (response) {
                    var res, dt;

                    var response = baseService.getExceptionProcessor_()(response);

                    if (!clazz_)
                        return response.data || null;

                    dt = getDate().getTime();
                    res = Serializer.deserialize(response.data, clazz_);
                    //_DEBUG && console.info('deserialize time', getDate().getTime() - dt);
                    return res;
                }
            },

            function getExceptionProcessor_(){
                return function (response){
                    if (response.success != true) {
                        var exceptionType = response.data.exceptiontype;
                        switch (exceptionType) {
                            case 'ChalkableSisException':
                                throw chlk.lib.exception.ChalkableSisException(response.data.message);
                            case 'ChalkableSisNotSupportVersionException':
                                throw chlk.lib.exception.ChalkableSisNotSupportVersionException(response.data.message);
                            case 'ChalkableException':
                                throw chlk.lib.exception.ChalkableException(response.data.message, null, response.data.title);
                            case 'NoAnnouncementException':
                                throw chlk.lib.exception.NoAnnouncementException(response.data.message);
                            case 'NoClassAnnouncementTypeException':
                                throw chlk.lib.exception.NoClassAnnouncementTypeException();
                            case 'AnnouncementDeleteFailedException':
                                throw chlk.lib.exception.AnnouncementDeleteFailedException(response.data.message, null, response.data.title);
                            case 'AggregateException':
                                throw  chlk.lib.exception.ChalkableSisException(response.data.innermessage);
                            default:
                                _DEBUG && console.error(exceptionType, response.data.message, response.stacktrace);
                                throw chlk.lib.exception.DataException(exceptionType + ': ' + response.data.message);
                        }
                    }
                    return response;
                }
            },

            function getPaginatedResponseProcessor_(clazz){
                var baseService = this;
                return function (response) {

                    response = baseService.getExceptionProcessor_()(response);

                    var model = new chlk.models.common.PaginatedList(clazz);
                    var dt = getDate().getTime();
                    model.setItems(Serializer.deserialize(response.data, ArrayOf(clazz)));
                    //_DEBUG && console.info('deserialize time', getDate().getTime() - dt);
                    model.setPageIndex(Number(response.pageindex));
                    model.setPageSize(Number(response.pagesize));
                    model.setActualCount(Number((response.data || []).length));
                    model.setTotalCount(Number(response.totalcount));
                    model.setTotalPages(Number(response.totalpages));
                    model.setHasNextPage(Boolean(response.hasnextpage));
                    model.setHasPreviousPage(Boolean(response.haspreviouspage));
                    return model;
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
                var validateResult = this.validateFiles_(files)
                return validateResult ||  new chlk.lib.ajax.UploadFileTask(this.resolveUri(uri), files)
                    .params(gParams_ || {})
                    .requestHeaders(this.prepareDefaultHeaders({}))
                    .run()
                    .then(this.getResponseProcessor_(clazz_));
            },

            ria.async.Future, function validateFiles_(files){
                var maxFileMbSize = 50;
                var maxFileSize = maxFileMbSize*1024*1024;
                var filesCount = files.filter(function(file){return file.size > maxFileSize;}).length;
                if(filesCount <= 0) return null;
                return new ria.async.DeferredData(null)
                        .then(function(data){
                            throw chlk.lib.exception.FileSizeExceedException('File exceed size limit ' + maxFileMbSize + ' MB');
                        })
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
                        "Authorization": "Signature:" + token
                    }))
                    .run();
            },

            [[String, Object, String, Object]],
            ria.async.Future, function makeGetPaginatedListApiCall(url, clazz, token, gParams){
                return  new chlk.lib.ajax.ChlkJsonGetTask(url)
                    .params(gParams)
                    .requestHeaders({
                        "Authorization": "Signature:" + token
                    })
                    .run()
                    .then(function(response){
                        if(response.success != true){
                            _DEBUG && console.error("ChalkableApiException", response.data.Message, response.data.StackTrace);
                            throw chlk.lib.exception.ChalkableApiException();
                        }
                        return response;
                    })
                    .then(this.getPaginatedResponseProcessor_(clazz));
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
                    .then(this.getPaginatedResponseProcessor_(clazz));
//                        throw(new Exception(handler.getMessage()));
            }
        ]);
});
