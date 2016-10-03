REQUIRE('ria.templates.CompiledTemplate');

NAMESPACE('chlk.templates', function () {
    "use strict";

    ASSET('~/assets/jade/render-with.jade')();

    /** @class chlk.templates.JadeTemplate */
    CLASS(
        'JadeTemplate', EXTENDS(ria.templates.CompiledTemplate), [
            Function, 'block'
        ])
});