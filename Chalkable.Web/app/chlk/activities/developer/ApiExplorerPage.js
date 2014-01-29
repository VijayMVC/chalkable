REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.developer.ApiExplorerTpl');
REQUIRE('chlk.templates.developer.ApiExplorerResponseTpl');
REQUIRE('chlk.templates.developer.ApiListTpl');
REQUIRE('chlk.templates.developer.ApiCallSeqTpl');
REQUIRE('chlk.models.api.ApiParamType');

NAMESPACE('chlk.activities.developer', function () {


    function serializeFormToJSON(obj){
      var arrayData, objectData;
      arrayData = obj.serializeArray();
      objectData = {};

      jQuery.each(arrayData, function() {
        var value;

        if (this.value != null) {
          value = this.value;
        } else {
          value = '';
        }

        if (objectData[this.name] != null) {
          if (!objectData[this.name].push) {
            objectData[this.name] = [objectData[this.name]];
          }

          objectData[this.name].push(value);
        } else {
          objectData[this.name] = value;
        }
      });

      return objectData;
    };

    /** @class chlk.activities.developer.ApiExplorerPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.developer.ApiExplorerTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.developer.ApiCallSeqTpl, 'update-api-calls-list', '.api-calls-seq', ria.mvc.PartialUpdateRuleActions.Replace)],
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

            [[ria.dom.Dom]],
            function refreshExampleCode(node){
                var tabs = node.parent();
                var tabContent = this.dom.find('#' + tabs.getAttr('tabId'));

                var form = this.dom.find('#' + tabContent.getAttr('formId'));
                var controllerName = form.find('input[name=controllerName]').getValue();
                var actionName = form.find('input[name=methodName]').getValue();

                var url = WEB_SITE_ROOT + controllerName + "/" + actionName + ".json";

                var formData = serializeFormToJSON(jQuery('#' + tabContent.getAttr('formId')));
                delete formData.apiFormId;
                delete formData.controllerName;
                delete formData.methodName;
                delete formData.apiCallRole;

                var params = JSON.stringify(formData);
                var codeArea = tabContent.find('pre');

                var code = this.getExampleCode_(node.getAttr('data-example-type') | 0, url, params, "{your token here}");
                codeArea.empty().setHTML(code);
                prettyPrint();
            },

            [ria.mvc.DomEventBind('keyup', '.value-field')],
            [[ria.dom.Dom, ria.dom.Event]],
            function onFieldChange(node, event){
              var form = this.dom.find('#' + node.getAttr('data-formid'));
              var tab = form.find('.tab-header.active');
              this.refreshExampleCode(tab);
            },

            [ria.mvc.DomEventBind('click', '.way-item')],
            [[ria.dom.Dom, ria.dom.Event]],
            function onNodeClick(node, event){
                var text = jQuery(node.valueOf()).data('param');
                var resItem;
                jQuery('.header-body .action').each(function(index, item){
                    if (jQuery(item).data('method-name') == text){
                        resItem = item;
                    }
                });

                if (resItem) {
                    jQuery(document).scrollTop(jQuery(resItem).parent().offset().top);
                    //think about this madness
                    if (!jQuery(resItem).parent().parent().parent().find('.details').is(':visible'))
                        resItem.click();
                }
            },



            [ria.mvc.DomEventBind('click', '.tab-header')],
            [[ria.dom.Dom, ria.dom.Event]],
            function exampleTabClick(node, event){
                var tabs = node.parent();
                tabs.find('.active').removeClass('active');
                this.refreshExampleCode(node);
                node.addClass('active');
            },

            function getExampleCode_(type, url, params, token){
                //type 0 == curl
                //type 1 == ruby
                //type 2 == python

                var result = "no example";

                switch(type){
                    case 0:{
                        var authHeader = ' -H "Authorization: Bearer:' + token + '"';
                        result = 'curl -X POST -H "Content-Type: application/json"\n' + authHeader + '\n -d ' + params + ' \n' + url;
                    }  break;
                    case 1:{

                        result = "import json import urllib2" + "\n" +

                        "data = {\n" +
                            "'ids': [12, 3, 4, 5, 6]";

                    } break;
                }
                return result;
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
            },

            [ria.mvc.DomEventBind('click', '.api-search-btn')],
            [[ria.dom.Dom, ria.dom.Event]],
            function apiSearchBtnClick(node, event) {
                 jQuery('#api-search-box').autocomplete( "search" , "");
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.developer.ApiExplorerResponseTpl)],
            VOID, function updateApiResponse(tpl, model, msg_) {
                var formId = model.getApiFormId();
                var form = this.dom.find('#' + formId);
                var responseContainer = form.find('.response');
                tpl.renderTo(responseContainer.empty());
                jQuery(form.valueOf()).find('pre.result').snippet("javascript", {style:"ide-eclipse"});
            }
        ]);
});