NAMESPACE('chlk.models.standard', function () {
    "use strict";

    /** @class chlk.models.standard.SelectedStandard*/
    CLASS(
        'SelectedStandard', [
            String, 'name',
            String, 'tooltip',
            String, 'id',
            String, 'description',

            [[String, String, String, String]],
            function $(id, name, description_, tooltip_){
                BASE();
                this.setId(id);
                this.setName(name);
                tooltip_ && this.setTooltip(tooltip_);
                description_ && this.setDescription(description_);
            }
        ]);
});
