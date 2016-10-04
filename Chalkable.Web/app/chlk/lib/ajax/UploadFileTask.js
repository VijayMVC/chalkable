REQUIRE('ria.ajax.Task');

NAMESPACE('chlk.lib.ajax', function () {
    "use strict";

    /** @class chlk.lib.ajax.UploadFileTask */
    CLASS(
        'UploadFileTask', EXTENDS(ria.ajax.Task), [
            [[String, Object]],
            function $(url, files) {
                BASE(url, ria.ajax.Method.POST);
                this._files = files;

                if (this._xhr.upload) {
                    this._xhr.upload.addEventListener("progress", this.updateProgress_, false);
                    this._xhr.upload.addEventListener("error", this.transferFailed_, false);
                    //this._xhr.upload.addEventListener("abort", this.transferCanceled_, false);
                }
            },

            OVERRIDE, ria.async.Future, function run() {
                return BASE()
                    .then(function (data) {
                        return JSON.parse(data);
                    })
            },

            OVERRIDE, VOID, function transferComplete_(evt) {
                if (this._xhr.status == 401)   {
                    this._completer.completeError(chlk.lib.exception.NotAuthorizedException());
                    return;
                }
                BASE(evt);
            },

            OVERRIDE, VOID, function do_() {
                try {
                    var formData = new FormData();

                      for (var i = 0, file; file = this._files[i]; ++i) {
                        formData.append(file.name, file);
                        for(var param in this._params)if (this._params.hasOwnProperty(param)) {
                          formData.append(param, this._params[param]);
                        }
                      }

                    if(!this._files.length){
                        for(var param in this._params)if (this._params.hasOwnProperty(param)) {
                            formData.append(param, this._params[param]);
                        }
                    }

                      this._xhr.open(this._method.valueOf(), this.getUrl_(), true);

                      this._xhr.send(formData);
                } catch (e) {
                    this._completer.completeError(e);
                }

                // todo change to ria.async.Timer.$once
                this._requestTimeout && new ria.async.Timer(this._requestTimeout, this.timeoutHandler_);
            }
        ]);
});