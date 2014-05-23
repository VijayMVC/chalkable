NAMESPACE('chlk.models.common.attachments', function () {
    "use strict";

    var HIDDEN_CLS = "x-hidden";
    /** @class chlk.models.common.attachments.ToolbarButton*/
    CLASS(
        'ToolbarButton', [
            String, 'title',
            String, 'id',
            String, 'url',
            Boolean, 'targetBlank',
            String, 'cls',

            [[String, String, String, Boolean]],
            function $(id, title, url_, targetBlank_){
                BASE();
                this.setId(id);
                this.setTitle(title);
                //this.setCls(HIDDEN_CLS);
                if (url_)
                    this.setUrl(url_);
                if (targetBlank_)
                    this.setTargetBlank(targetBlank_);
            }
        ]);
});
