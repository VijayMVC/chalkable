REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.attachment.Attachment');

NAMESPACE('chlk.templates.attach', function(){

    /**@class chlk.templates.attach.AttachmentTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/attach/attachment.jade')],
        [ria.templates.ModelBind(chlk.models.attachment.Attachment)],
        'AttachmentTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AttachmentId, 'id',

            [ria.templates.ModelPropertyBind],
            String, 'name',

            [ria.templates.ModelPropertyBind],
            String, 'thumbnailUrl',

            [ria.templates.ModelPropertyBind],
            chlk.models.attachment.AttachmentTypeEnum, 'type',

            [ria.templates.ModelPropertyBind],
            String, 'url',

            [ria.templates.ModelPropertyBind],
            Number, 'fileIndex',

            [ria.templates.ModelPropertyBind],
            Number, 'total',

            [ria.templates.ModelPropertyBind],
            Number, 'loaded',

            function getSize(){
                var texts = ['b', 'kb', 'mb', 'gb'], step = 0;
                var total = this.getTotal();
                while(total > 1023){
                    step++;
                    total = parseFloat((total / 1024).toFixed(2));
                }
                return total + texts[step];
            }
    ]);
});
