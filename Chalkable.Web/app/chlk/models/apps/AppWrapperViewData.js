REQUIRE('chlk.models.apps.Application');
REQUIRE('chlk.models.apps.AppModes');

NAMESPACE('chlk.models.apps', function () {
    "use strict";



    /** @class chlk.models.apps.AppWrapperToolbarButton*/
    CLASS(
        'AppWrapperToolbarButton', [
            String, 'title',
            String, 'id',
            String, 'url',
            Boolean, 'targetBlank',

            [[String, String, String, Boolean]],
            function $(id, title, url_, targetBlank_){
                this.setId(id);
                this.setTitle(title);
                if (url_)
                    this.setUrl(url_);
                if (targetBlank_)
                    this.setTargetBlank(targetBlank_);
            }
        ]);

    /** @class chlk.models.apps.AppWrapperViewData*/
    CLASS(
        'AppWrapperViewData', [
            chlk.models.apps.AppModes, 'appMode',
            ArrayOf(chlk.models.apps.AppWrapperToolbarButton), 'buttons',
            chlk.models.apps.Application, 'app',

            [[chlk.models.apps.Application, chlk.models.apps.AppModes, ArrayOf(chlk.models.apps.AppWrapperToolbarButton)]],
            function $(app, mode, buttons){
                this.setApp(app);
                this.setAppMode(mode);
                this.setButtons(buttons);
            }
        ]);
});
