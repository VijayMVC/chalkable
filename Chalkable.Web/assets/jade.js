ria.__REQUIRE.addPlugin(
    'lib/jade.js',
    'ria.ajax.Task',

    function () {
        "use strict";

        ria.__REQUIRE.addAssetAlias('ria.templates.TemplateBind');

        ria.__REQUIRE.addLoader(
            function filter(path) {
                return /\.jade$/.test(path);
            },

            function loader(src, callback) {
                new ria.ajax.Task(src)
                    .method(ria.ajax.Method.GET)
                    .run()
                        .then(function(content) {
                            var fn = jade.compile(content, {self: true, compileDebug: true, filename: src});
                            callback(function () {
                                try {
                                    return fn.apply(this, arguments);
                                } catch (e) {
                                    if (e instanceof Error)
                                        e = new Exception(e.message, e);
                                    throw new Exception('Error rendering jade "' + src + '"', e);
                                }
                            }, null);
                        })
                        .catchError(function (error) {
                            callback(null, error);
                            setTimeout(function () {
                                throw new Exception('Error compiling "' + src + '"', error);
                            }, 1)
                        });
            }
        )
    });