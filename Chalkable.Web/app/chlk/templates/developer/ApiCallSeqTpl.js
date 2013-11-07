REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.api.ApiCallSequence');

NAMESPACE('chlk.templates.developer', function () {

    /** @class chlk.templates.developer.ApiCallSeqTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/developer/apiCallSeq.jade')],
        [ria.templates.ModelBind(chlk.models.api.ApiCallSequence)],
        'ApiCallSeqTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.api.ApiListItem), 'items',

            [[ArrayOf(chlk.models.api.ApiListItem)]],
            function getApiCallSequence(data){
                var callTreeItems = [];
                var extLength = data.length;
                if (data.length > 1) extLength += (data.length - 1);
                var isEven = extLength % 2 == 0;
                var currIndex = -1;

                for(var i = 0; i < extLength; ++i){
                  if (isEven){
                    if(i % 2 == 0){
                     callTreeItems.push({separator: true});
                    }
                    else{
                      ++currIndex;
                      var item = data[currIndex];
                      item.separator = false;
                      item.setIndex(currIndex + 1);
                      callTreeItems.push(item);
                    }
                  }
                  else{
                    if(i % 2 != 0){
                     callTreeItems.push({separator: true});
                    }
                    else{
                      ++currIndex;
                      var item = data[currIndex];
                      item.setIndex(currIndex + 1);
                      item.separator = false;
                      callTreeItems.push(item);
                    }
                  }
                }

                return callTreeItems;
            }
        ])
});



