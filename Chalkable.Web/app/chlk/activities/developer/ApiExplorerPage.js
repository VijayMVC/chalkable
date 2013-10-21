REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.developer.ApiExplorerTpl');
REQUIRE('chlk.models.api.ApiParamType');

NAMESPACE('chlk.activities.developer', function () {

    /** @class chlk.activities.developer.ApiExplorerPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.developer.ApiExplorerTpl)],
        'ApiExplorerPage', EXTENDS(chlk.activities.lib.TemplatePage), [

            [ria.mvc.DomEventBind('click', '.header')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function toggleDetails(node, event){
                node.find('.description').toggleClass('long');
                jQuery(node.parent().find('.details').valueOf()).slideToggle();
            },

            [ria.mvc.DomEventBind('click', '.collapse-all')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function collapseAll(node, event){
                var parentNode = jQuery(node.valueOf()).parent().parent();
                parentNode.find('.details').slideUp();
                parentNode.find('.header').find('.description').removeClass('long');
            },

            function getValidationDataForField(field){
                var paramType = field.data('param-type');

                var rule = {
                    required: !field.hasClass('optional'),
                    digits: paramType === chlk.models.api.ApiParamType.INTEGER.valueOf(),
                    date: paramType === chlk.models.api.ApiParamType.DATE.valueOf(),
                    intlist: paramType === chlk.models.api.ApiParamType.INTLIST.valueOf(),
                    guid: paramType === chlk.models.api.ApiParamType.GUID.valueOf(),
                    guidlist: paramType === chlk.models.api.ApiParamType.GUIDLIST.valueOf()
                };

                var message = {
                    required: " *",
                    digits: "only integers are allowed",
                    date: "invalid date format",
                    intlist: "only comma separated integers are allowed"
                };

                return {
                    rule: rule,
                    message: message
                };
            },

            [ria.mvc.DomEventBind('click', '.try-btn')],
            [[ria.dom.Dom, ria.dom.Event]],
            function tryBtnClick(node, event) {
               var formId = node.parent().getAttr('id');
               var formData = {};
               var rules = {};
               var messages = {};
               var form = jQuery('#' + formId);

               var that = this;

               form.find('.value-field').each(function(field){
                    var fieldName = jQuery(this).attr('name');
                    formData[fieldName] = jQuery(this).val();
                    var validationData = that.getValidationDataForField(jQuery(this));
                    rules[fieldName] = validationData.rule;
                    messages[fieldName] = validationData.message;
               });

               form.validate({
                   rules: rules,
                   messages: messages
               });

               return form.valid();
            }
        ]);
});