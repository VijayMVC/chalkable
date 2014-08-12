REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

    /** @class chlk.controls.VideoControl */
    CLASS(
        'VideoControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/video.jade')(this);
                this.setConfigs({
                    wmode: 'transparent',
                    allowScriptAccess: 'always',
                    allowfullscreen: 'true',
                    frameborder: 0,
                    vspace: 0,
                    hspace: 0
                })
            },

            Object, 'configs',

            [[String]],
            String, function getVideoUrl(url){
                if(url.indexOf('vimeo') > -1 || url.indexOf('/') == -1){
                    url = 'https://player.vimeo.com/video/' + (url && url.slice(url.lastIndexOf('/') + 1));
                }
                if(url.indexOf('youtube') > -1){
                    url = 'https://www.youtube.com/v/' + (url && url.slice(url.lastIndexOf('v=') + 2));
                }
                return url;
            },

            [[Object]],
            Object, function processAttrs(attributes){
                attributes.src = this.getVideoUrl(attributes.src);
                attributes = ria.__API.extendWithDefault(attributes, this.getConfigs());
                return attributes;
            }
        ]);
});