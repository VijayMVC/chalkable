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
            String, 'action',
            String, 'controller',
            Array, 'params',
            Boolean, 'right',

            [[String, String, String, Boolean, String, String, Array, Boolean]],
            function $(id, title, url_, targetBlank_, controller_, action_, params_, right_){
                BASE();
                this.setId(id);
                this.setTitle(title);
                //this.setCls(HIDDEN_CLS);
                if (url_)
                    this.setUrl(url_);
                if (targetBlank_)
                    this.setTargetBlank(targetBlank_);
                if (action_)
                    this.setAction(action_);
                if (controller_)
                    this.setController(controller_);
                if (params_)
                    this.setParams(params_);
                if (right_)
                    this.setRight(right_);
            }
        ]);
});
