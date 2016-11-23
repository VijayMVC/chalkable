REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.reports.CustomReportTemplateDialogTpl');

NAMESPACE('chlk.activities.reports', function () {
    /** @class chlk.activities.reports.CustomReportTemplateDialog */
    CLASS(
        [ria.mvc.ActivityGroup('ReportTemplateDialog')],
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.reports.CustomReportTemplateDialogTpl)],
        'CustomReportTemplateDialog', EXTENDS(chlk.activities.lib.TemplateDialog), [

            function handleFileSelect_(evt) {
                var files = evt.target.files; // FileList object

                // Loop through the FileList and render image files as thumbnails.
                for (var i = 0, f; f = files[i]; i++) {

                    // Only process image files.
                    if (!f.type.match('image.*')) {
                        continue;
                    }

                    var reader = new FileReader();

                    // Closure to capture the file information.
                    reader.onload = (function(theFile) {
                        //ria.dom.Dom('.template-img').setAttr('src', evt.target.result);
                        return function(e) {
                            // Render thumbnail.
                            ria.dom.Dom('.template-img').setAttr('src', e.target.result);
                            /*var span = document.createElement('span');
                            span.innerHTML = ['<img class="thumb" src="', e.target.result,
                                '" title="', escape(theFile.name), '"/>'].join('');
                            document.getElementById('list').insertBefore(span, null);*/
                        };
                    })(f);

                    // Read in the image file as a data URL.
                    reader.readAsDataURL(f);
                }
            },

            [ria.mvc.DomEventBind('change', '#icon')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function iconChange(node, event, selected_){
                this.handleFileSelect_(event);
            }
        ]);
});
