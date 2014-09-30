REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

    /** @class chlk.controls.ActionLinkControl */
    CLASS(
        'FileUploadControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/file-upload.jade')(this);
            },

            [[Object]],
            Object, function prepareData(attrs) {
                attrs.id = attrs.id || ria.dom.Dom.GID();
                var that = this, params = attrs['data-params'] || [],
                    controller = attrs['data-controller'],
                    action = attrs['data-action'];
                if (controller)
                {
                    this.context.getDefaultView()
                        .onActivityRefreshed(function (activity, model) {
                            var node = ria.dom.Dom('#' + attrs.id);
                            node.on('change', function(target, event){
                                var files = target.valueOf()[0].files;
                                var state = that.context.getState();
                                params.push(files);
                                state.setController(controller);
                                state.setAction(action);
                                state.setParams(params);
                                state.setPublic(false);
                                that.context.stateUpdated();
                            })
                        }.bind(this));
                }
                return attrs;
            }
        ]);
});