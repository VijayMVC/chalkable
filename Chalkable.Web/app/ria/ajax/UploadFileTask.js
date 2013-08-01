REQUIRE('ria.ajax.Task');

NAMESPACE('ria.ajax', function () {
    "use strict";

    /** @class ria.ajax.UploadFileTask */
    CLASS(
        'UploadFileTask', EXTENDS(ria.ajax.Task), [
            [[String, Object]],
            function $(url, files) {
                BASE(url, ria.ajax.Method.POST);
                this._files = files;
            },

            OVERRIDE, ria.async.Future, function run() {
                return BASE()
                    .then(function (data) {
                        return JSON.parse(data);
                    })
            },

            OVERRIDE, VOID, function do_() {
                try {
                    var formData = new FormData();

                      for (var i = 0, file; file = this._files[i]; ++i) {
                        formData.append(file.name, file);
                      }

                      var xhr = new XMLHttpRequest();
                      xhr.open(this._method.valueOf(), this.getUrl_(), true);

                      xhr.send(formData);
                } catch (e) {
                    this._completer.completeError(e);
                }

                // todo change to ria.async.Timer.$once
                this._requestTimeout && new ria.async.Timer(this._requestTimeout, this.timeoutHandler_);
            }
        ]);
});