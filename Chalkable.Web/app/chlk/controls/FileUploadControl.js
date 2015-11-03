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
                    action = attrs['data-action'],
                    needFileIndex = attrs.needFileIndex,
                    index = 0,
                    dropAreaSelector = attrs.dropAreaSelector;
                if (controller)
                {
                    this.context.getDefaultView()
                        .onActivityRefreshed(function (activity, model) {
                            var node = ria.dom.Dom('#' + attrs.id);
                            node.on('change', function(target, event){
                                var files = target.valueOf()[0].files;
                                var state = that.context.getState();
                                var p = params.slice();
                                if(needFileIndex){
                                    p.push(index);
                                    index = index + files.length;
                                }

                                p.push(files);
                                state.setController(controller);
                                state.setAction(action);
                                state.setParams(p);
                                state.setPublic(false);
                                that.context.stateUpdated();
                            });

                            var $dropArea = dropAreaSelector ? node.$.closest(dropAreaSelector) : node.$;
                            $dropArea
                                .on('dragenter', function (e) {
                                    e.stopPropagation();
                                    e.preventDefault();
                                    dropAreaSelector && $dropArea.css({'border-style': 'solid'});
                                })
                                .on('dragover', function (e) {
                                    e.stopPropagation();
                                    e.preventDefault();
                                })
                                .on('dragleave', function (e) {
                                    e.stopPropagation();
                                    e.preventDefault();

                                    if (dropAreaSelector && !$.contains(this, e.target))
                                        $dropArea.css({'border-style': ''});
                                })
                                .on('drop', function (e) {
                                    e.preventDefault();

                                    dropAreaSelector && $dropArea.css({'border-style': ''});

                                    var files = e.originalEvent.dataTransfer.files;

                                    var state = that.context.getState();
                                    var p = params.slice();
                                    if(needFileIndex){
                                        p.push(index);
                                        index = index + files.length;
                                    }
                                    p.push(files);
                                    state.setController(controller);
                                    state.setAction(action);
                                    state.setParams(p);
                                    state.setPublic(false);
                                    that.context.stateUpdated();
                                });

                        }.bind(this));
                }
                return attrs;
            }
        ]);
});
