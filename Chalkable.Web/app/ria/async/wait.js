REQUIRE('ria.async.Completer');

NAMESPACE('ria.async', function () {
    "use strict";

    /**
     * @param {ria.async.Future} future...
     * @returns {ria.async.Future}
     */
    ria.async.wait = function (future) {
        var futures = Array.isArray(future) ? future : [].slice.call(arguments)
          , completer = new ria.async.Completer
          , counter = 0
          , size = futures.length + 1
          , results = []
          , complete = false;

        futures.unshift(ria.async.DeferredAction()); // just in case futures are empty or all resolved :)

        futures.forEach(function (_, index) {
            VALIDATE_ARG('future', [ria.async.Future], _);

            _.then(function (data) {
                if (complete) return ;

                counter++;
                results[index] = data;

                if (counter == size) {
                    complete = true;
                    completer.complete(data);
                } else {
                    completer.progress(counter);
                }
            }).catchError(function (e) {
                if (complete) return ;

                complete = true;
                completer.completeError(e);
            })
        });

        return completer.getFuture();
    };
})