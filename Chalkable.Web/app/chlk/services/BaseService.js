REQUIRE('chlk.lib.serialize.ChlkJsonSerializer');

REQUIRE('chlk.lib.ajax.ChlkJsonPostTask');
REQUIRE('ria.ajax.JsonGetTask');
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
                return this.getContext().getSession().get('siteRoot');
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

            [[Object]],
            function handleError(data){
                if(!data.success){
                    this.redirectToErrorPage();
                }
                //throw chlk.services.DataException('Server error', Error(data.message));
            },

            function redirectToErrorPage(){
                var state = this.context.getState();
                state.setController('error');
                state.setAction('generalError');
                state.setParams([]);
                state.setPublic(false);
                this.context.stateUpdated();
            },

            [[String, Object, Object]],
            ria.async.Future, function get(uri, clazz_, gParams_) {

                return new ria.ajax.JsonGetTask(this.resolveUri(uri))
                    .params(gParams_ || {})
                    .run()
                    .then(function (data) {
                        this.handleError(data);

                        if (!clazz_)
                            return data.data || null;

                        return Serializer.deserialize(data.data, clazz_);
                    }.bind(this)).catchError(function(handler, scope_){
                        this.redirectToErrorPage();
                    }, this);
            },

            [[String, Object, Object, Object]],
            ria.async.Future, function uploadFiles(uri, files, clazz_, gParams_) {

                return new chlk.lib.ajax.UploadFileTask(this.resolveUri(uri), files)
                    .params(gParams_ || {})
                    .run()
                    .then(function (data) {
                        this.handleError(data);

                        if (!clazz_)
                            return data.data || null;

                        return Serializer.deserialize(data.data, clazz_);
                    }.bind(this));
            },


            [[String, Object, Object]],
            ria.async.Future, function post(uri, clazz, gParams) {

                return new chlk.lib.ajax.ChlkJsonPostTask(this.resolveUri(uri))
                    .params(gParams)
                    .requestHeaders({"Content-Type": "application/json; charset=utf-8"})
                    .run()
                    .then(function (data) {
                        this.handleError(data);

                        return Serializer.deserialize(data.data, clazz);
                    }.bind(this));
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
                    .requestHeaders({
                        "Content-Type": "application/json; charset=utf-8",
                        "Authorization": "Bearer:" + token
                    })
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
                    .requestHeaders({"Content-Type": "application/json; charset=utf-8"})
                    .run()
                    .then(function (data) {
                        this.handleError(data);

                        return Serializer.deserialize(data.data, ArrayOf(clazz));
                    }.bind(this));
            },

            [[String, Object, Object]],
            ria.async.Future, function getArray(uri, clazz, gParams) {

                return new ria.ajax.JsonGetTask(this.resolveUri(uri))
                    .params(gParams)
                    .run()
                    .then(function (data) {
                        this.handleError(data);

                        return Serializer.deserialize(data.data, ArrayOf(clazz));
                    }.bind(this)).catchError(function(handler, scope_){
                        this.redirectToErrorPage();
                    }, this);
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
                    }).catchError(function(handler, scope_){
                        this.redirectToErrorPage();
                    }, this);
            }
        ]);
});
